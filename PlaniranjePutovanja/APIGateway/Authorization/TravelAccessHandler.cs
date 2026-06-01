using Microsoft.AspNetCore.Authorization;
using PlaniranjePutovanja.Common.Enums;
using PlaniranjePutovanja.Common.Interfaces.Travel;
using System.Security.Claims;

namespace PlaniranjePutovanja.APIGateway.Authorization
{
    public sealed class TravelAccessHandler : AuthorizationHandler<TravelAccessRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITravelService _travelService;

        public TravelAccessHandler(IHttpContextAccessor httpContextAccessor, ITravelService travelService)
        {
            _httpContextAccessor = httpContextAccessor;
            _travelService = travelService;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TravelAccessRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            // 1. Pravilo: Admin moze SVE
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            // Izvuci ID ulogovanog korisnika iz tokena
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

            // =========================================================================
            // AKO NEMA travelId-a (Npr. pretraga putovanja po userId)
            // =========================================================================
            if (string.IsNullOrEmpty(travelId))
            {
                // Ako je ruta oblika /api/travel/travels/user/{userId}
                if (httpContext.Request.RouteValues.TryGetValue("userId", out var uId) && uId != null)
                {
                    var requestedUserId = uId.ToString();

                    // Da li ulogovani korisnik trazi svoja sopstvena putovanja?
                    if (!string.IsNullOrEmpty(currentUserId) && currentUserId == requestedUserId)
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }

                context.Fail();
                return;
            }

            // =========================================================================
            // PROVERA (KADA POSTOJI travelId U RUTI)
            // =========================================================================

            // 2. Proveri da li je obican korisnik ustvari vlasnik putovanja
            if (!string.IsNullOrEmpty(currentUserId))
            {
                var travel = await _travelService.GetTravelByIdAsync(travelId);
                if (travel != null && travel.UserId == currentUserId)
                {
                    // Vlasnik - Puna prava
                    context.Succeed(requirement);
                    return;
                }
            }

            // 3. Proveri Share Token (Gost sa linkom QR koda)
            var tokenTravelId = context.User.FindFirst("TravelId")?.Value;
            var tokenAccessLevel = context.User.FindFirst("AccessLevel")?.Value;

            if (!string.IsNullOrEmpty(tokenTravelId) && tokenTravelId == travelId)
            {
                if (Enum.TryParse<ShareAccessLevel>(tokenAccessLevel, out var level))
                {
                    if (requirement.RequiredAccess == AccessRequirement.View &&
                       (level == ShareAccessLevel.View || level == ShareAccessLevel.Edit))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                    if (requirement.RequiredAccess == AccessRequirement.Edit && level == ShareAccessLevel.Edit)
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }

            // Odbij pristup ako gornje ne prođe
            context.Fail();
        }
        //protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TravelAccessRequirement requirement)
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    if (httpContext == null)
        //    {
        //        context.Fail();
        //        return;
        //    }

        //    // 1. Pravilo - Admin moze SVE
        //    if (context.User.IsInRole("Admin"))
        //    {
        //        context.Succeed(requirement);
        //        return;
        //    }

        //    // Izvucemo 'travelId' iz rute npr. "/api/travel/travels/{travelId}"
        //    string? travelId = null;
        //    if (httpContext.Request.RouteValues.TryGetValue("travelId", out var rv) && rv != null)
        //    {
        //        travelId = rv.ToString();
        //    }
        //    else if (httpContext.Request.RouteValues.TryGetValue("id", out var idv) && idv != null) // Za rute koje koriste {id}
        //    {
        //        travelId = idv.ToString();
        //    }
        //    else if (httpContext.Request.Query.TryGetValue("travelId", out var qv))
        //    {
        //        travelId = qv.FirstOrDefault();
        //    }

        //    if (string.IsNullOrEmpty(travelId))
        //    {
        //        // Nema id-a putovanja u ruti, ne mozemo autorizovati po putovanju
        //        context.Fail();
        //        return;
        //    }

        //    // Proverimo Share Token (da li je ovo token iz UtilService)
        //    // Share tokeni nemaju ulogovanog korisnika (nemaju userId), vec imaju TravelId i AccessLevel claim-ove
        //    var tokenTravelId = context.User.FindFirst("TravelId")?.Value;
        //    var tokenAccessLevel = context.User.FindFirst("AccessLevel")?.Value;

        //    if (!string.IsNullOrEmpty(tokenTravelId) && tokenTravelId == travelId)
        //    {
        //        if (Enum.TryParse<ShareAccessLevel>(tokenAccessLevel, out var level))
        //        {
        //            // VIEW pristup dozvoljava samo VIEW
        //            // EDIT dozvoljava i EDIT i VIEW
        //            if (requirement.RequiredAccess == AccessRequirement.View &&
        //               (level == ShareAccessLevel.View || level == ShareAccessLevel.Edit))
        //            {
        //                context.Succeed(requirement);
        //                return;
        //            }
        //            if (requirement.RequiredAccess == AccessRequirement.Edit && level == ShareAccessLevel.Edit)
        //            {
        //                context.Succeed(requirement);
        //                return;
        //            }
        //        }
        //        context.Fail(); // Fail ako claim ne odgovara trazenom pravu
        //        return;
        //    }

        //    // Proverimo standardnog ulogovanog korisnika (da li je on vlasnik putovanja)
        //    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
        //                 context.User.FindFirst("userId")?.Value;

        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        context.Fail();
        //        return;
        //    }

        //    // Pozovamo TravelService da proverimo ko je vlasnik
        //    var travel = await _travelService.GetTravelByIdAsync(travelId);
        //    if (travel == null)
        //    {
        //        context.Fail();
        //        return;
        //    }

        //    if (travel.UserId == userId)
        //    {
        //        // Vlasnik ima puna prava (i View i Edit)
        //        context.Succeed(requirement);
        //        return;
        //    }

        //    context.Fail();
        //}
    }
}
