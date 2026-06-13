using PlaniranjePutovanja.Common.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.Services
{
    public interface IAuthBusinessService
    {
        Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

        Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        
        Task<bool> DeleteUserAsync(string userId);
    }
}
