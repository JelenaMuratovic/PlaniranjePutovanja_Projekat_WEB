using Microsoft.ServiceFabric.Services.Remoting;
using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.Common.DTOs.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.Interfaces.Util
{
    public interface IUtilService : IService
    {
        /// <summary>
        /// Generise PDF izvestaj plana putovanja na osnovu ID-a putovanja
        /// </summary>
        Task<GeneratedFileDto> GenerateTravelPlanPdfAsync(string travelId);

        /// <summary>
        /// Kreira JWT za pristup putovanju i generise QR kod koji linkuje na taj token
        /// </summary>
        Task<ShareLinkResponseDto> GenerateShareQrCodeAsync(string travelId, GenerateShareLinkRequestDto request);

        /// <summary>
        /// Preuzima deljeno putovanje na osnovu tokena
        /// </summary>
        Task<TravelDto> GetSharedTravelByTokenAsync(string token);

        /// <summary>
        /// Validira share token i vraca podatke potrebne za autorizaciju u API Gateway-u
        /// </summary>
        Task<ShareTokenValidationDto?> ValidateShareTokenAsync(string token);
    }
}
