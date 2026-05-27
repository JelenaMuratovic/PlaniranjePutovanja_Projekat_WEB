using PlaniranjePutovanja.Common.DTOs.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService.Services
{
    public interface IUtilBusinessService
    {
        Task<GeneratedFileDto> GenerateTravelPlanPdfAsync(string travelId);

        Task<ShareLinkResponseDto> GenerateShareQrCodeAsync(string travelId, GenerateShareLinkRequestDto request);
    }
}
