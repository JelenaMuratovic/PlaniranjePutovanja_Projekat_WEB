using Microsoft.AspNetCore.Authorization;
using PlaniranjePutovanja.Common.Enums;
using PlaniranjePutovanja.Common.Interfaces.Travel;
using PlaniranjePutovanja.Common.Interfaces.Util;
using System.Security.Claims;

namespace PlaniranjePutovanja.APIGateway.Authorization
{
    public sealed class TravelAccessHandler : AuthorizationHandler<TravelAccessRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITravelService _travelService;
        private readonly IUtilService _utilService;

        public TravelAccessHandler(IHttpContextAccessor httpContextAccessor, ITravelService travelService, IUtilService utilService)
        {
            _httpContextAccessor = httpContextAccessor;
            _travelService = travelService;
            _utilService = utilService;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TravelAccessRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            // 1. Pravilo: Admin moze sve
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            var currentUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                context.User.FindFirst("userId")?.Value;

            // Pokusavamo da izvucemo 'travelId' iz rute
            string? travelId = null;
            if (httpContext.Request.RouteValues.TryGetValue("travelId", out var rv) && rv != null)
            {
                travelId = rv.ToString();
            }
            else if (httpContext.Request.RouteValues.TryGetValue("id", out var idv) && idv != null)
            {
                travelId = idv.ToString();
            }
            else if (httpContext.Request.Query.TryGetValue("travelId", out var qv))
            {
                travelId = qv.FirstOrDefault();
            }

            // Ako nema travelId-a 
            if (string.IsNullOrEmpty(travelId))
            {
                // Ako je ruta oblika /api/travel/travels/user/{userId}
                if (httpContext.Request.RouteValues.TryGetValue("userId", out var uId) && uId != null)
                {
                    var requestedUserId = uId.ToString();

                    // Da li ulogovani korisnik trazi svoja sopstvena putovanja
                    if (!string.IsNullOrEmpty(currentUserId) && currentUserId == requestedUserId)
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }

                context.Fail();
                return;
            }

            // Provera kada postoji tarvelId u ruti
            // Provera da li je obican korisnik vlasnik putovanja
            if (!string.IsNullOrEmpty(currentUserId))
            {
                var travel = await _travelService.GetTravelByIdAsync(travelId);
                if (travel != null && travel.UserId == currentUserId)
                {
                    // Vlasnik - puna prava
                    context.Succeed(requirement);
                    return;
                }
            }

            // Proveravamo Share Token (citamo ga iz custom headera koji salje frontend)
            string? tokenTravelId = null;
            string? tokenAccessLevel = null;

            // Proveravamo da li je frontend poslao X-Share-Token kroz header
            if (httpContext.Request.Headers.TryGetValue("X-Share-Token", out var shareTokenHeader) &&
                !string.IsNullOrWhiteSpace(shareTokenHeader))
            {
                try
                {
                    var validationResult = await _utilService.ValidateShareTokenAsync(shareTokenHeader.ToString());

                    if (validationResult != null)
                    {
                        tokenTravelId = validationResult.TravelId;
                        tokenAccessLevel = validationResult.AccessLevel.ToString();
                    }
                }
                catch (Exception)
                {
                    context.Fail();
                    return;
                }
            }
            else
            {
                // Fallback ako su claimovi direktno u korisniku
                tokenTravelId = context.User.FindFirst("TravelId")?.Value;
                tokenAccessLevel = context.User.FindFirst("AccessLevel")?.Value;
            }

            // Ako smo uspesno izvukli TravelId iz deljenog tokena i on se poklapa sa trazenim putovanjem
            if (!string.IsNullOrEmpty(tokenTravelId) && tokenTravelId == travelId)
            {
                if (Enum.TryParse<ShareAccessLevel>(tokenAccessLevel, out var level))
                {
                    // Ako se trazi samo pregled (VIEW), a token je VIEW ili EDIT -> Prolazi
                    if (requirement.RequiredAccess == AccessRequirement.View &&
                       (level == ShareAccessLevel.View || level == ShareAccessLevel.Edit))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                    // Ako se trazi izmena (EDIT), token MORA biti EDIT -> Prolazi
                    if (requirement.RequiredAccess == AccessRequirement.Edit && level == ShareAccessLevel.Edit)
                    {
                        // Ako pravilo nalaze da EDIT moze raditi samo ulogovan korisnik
                        var isUserLoggedIn = context.User.Identity?.IsAuthenticated == true || !string.IsNullOrEmpty(currentUserId);
                        if (isUserLoggedIn)
                        {
                            context.Succeed(requirement);
                            return;
                        }
                    }
                }
            }

            // Ako nijedna provera nije prosla, odbij zahtev
            context.Fail();

        }
    }
}
