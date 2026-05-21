using PlaniranjePutovanja.AuthService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.Helpers.Tokens
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);

        bool ValidateToken(string token);

        string? GetUserIdFromToken(string token);
    }
}
