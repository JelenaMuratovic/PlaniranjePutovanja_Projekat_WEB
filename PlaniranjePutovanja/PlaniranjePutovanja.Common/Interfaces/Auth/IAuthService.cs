using Microsoft.ServiceFabric.Services.Remoting;
using PlaniranjePutovanja.Common.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.Interfaces.Auth
{
    public interface IAuthService : IService
    {
        /// <summary>
        /// Registracija novog korisnika
        /// </summary>
        Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto request);

        /// <summary>
        /// Prijava korisnika
        /// </summary>
        Task<AuthResponseDto> LoginUserAsync(LoginRequestDto request);
        /// <summary>
        /// Admin akcija za prikaz svih korisnika
        /// </summary>
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        /// <summary>
        /// Admin akcija za brisanje korisnika
        /// </summary>
        /// <param name="userId"></param>
        Task<bool> DeleteUserAsync(string userId);
    }
}
