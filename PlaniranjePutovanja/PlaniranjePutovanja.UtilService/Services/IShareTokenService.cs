using PlaniranjePutovanja.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService.Services
{
    public interface IShareTokenService
    {
        /// <summary>
        /// Generise JWT token za deljenje putovanja sa specificiranim nivoima pristupa
        /// </summary>
        Task<string> GenerateShareTokenAsync(string travelId, ShareAccessLevel accessLevel, int expirationDays);

        /// <summary>
        /// Validira token i vraca TravelId i AccessLevel iz njega
        /// </summary>
        Task<(string TravelId, ShareAccessLevel AccessLevel)?> ValidateShareTokenAsync(string token);
    }
}
