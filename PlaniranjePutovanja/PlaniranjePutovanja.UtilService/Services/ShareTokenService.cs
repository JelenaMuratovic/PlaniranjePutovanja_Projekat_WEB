using Microsoft.IdentityModel.Tokens;
using PlaniranjePutovanja.Common.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService.Services
{
    public sealed class ShareTokenService : IShareTokenService
    {
        private readonly string _jwtSecretKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public ShareTokenService(string jwtSecretKey, string jwtIssuer, string jwtAudience)
        {
            _jwtSecretKey = jwtSecretKey;
            _jwtIssuer = jwtIssuer;
            _jwtAudience = jwtAudience;
        }

        public async Task<string> GenerateShareTokenAsync(string travelId, ShareAccessLevel accessLevel, int expirationDays)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim("TravelId", travelId),
            new Claim("AccessLevel", accessLevel.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expirationDays),
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return await Task.FromResult(tokenHandler.WriteToken(token));
        }

        public async Task<(string TravelId, ShareAccessLevel AccessLevel)?> ValidateShareTokenAsync(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var travelIdClaim = principal.FindFirst("TravelId")?.Value;
                var accessLevelClaim = principal.FindFirst("AccessLevel")?.Value;

                if (travelIdClaim == null || accessLevelClaim == null)
                    return null;

                if (!Enum.TryParse<ShareAccessLevel>(accessLevelClaim, out var accessLevel))
                    return null;

                return await Task.FromResult((travelIdClaim, accessLevel));
            }
            catch
            {
                return null;
            }
        }


    }
}
