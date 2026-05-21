using PlaniranjePutovanja.AuthService.Models;
using PlaniranjePutovanja.Common.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.Mappers
{
    public sealed class AuthMapper : IAuthMapper
    {
        public User MapToUser(RegisterRequestDto request, string passwordHash)
        {
            return new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash
            };
        }

        public UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}
