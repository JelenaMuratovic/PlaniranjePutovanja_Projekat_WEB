using PlaniranjePutovanja.AuthService.Models;
using PlaniranjePutovanja.Common.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.Mappers
{
    public interface IAuthMapper
    {
        User MapToUser(RegisterRequestDto request, string passwordHash);

        UserDto MapToUserDto(User user);
    }
}
