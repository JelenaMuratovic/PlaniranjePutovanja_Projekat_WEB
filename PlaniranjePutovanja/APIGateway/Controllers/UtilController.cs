using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlaniranjePutovanja.Common.DTOs.Util;
using PlaniranjePutovanja.Common.Interfaces.Util;

namespace PlaniranjePutovanja.APIGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UtilController : ControllerBase
    {
        private readonly IUtilService _utilService;
        private readonly ILogger<UtilController> _logger;

        public UtilController(IUtilService utilService, ILogger<UtilController> logger)
        {
            _utilService = utilService;
            _logger = logger;
        }

        [HttpPost("travels/{travelId}/share")]
        [Authorize(Policy = "CanEditTravel")]
        public async Task<IActionResult> GenerateShare(string travelId, [FromBody] GenerateShareLinkRequestDto request)
        {
            try
            {
                var result = await _utilService.GenerateShareQrCodeAsync(travelId, request);
                return Created($"api/util/travels/{travelId}/share", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generate share failed.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("travels/{travelId}/pdf")]
        [Authorize(Policy = "CanViewTravel")]
        public async Task<IActionResult> GetPdf(string travelId)
        {
            try
            {
                var file = await _utilService.GenerateTravelPlanPdfAsync(travelId);
                if (file == null || file.FileContents == null || file.FileContents.Length == 0)
                {
                    return NotFound();
                }

                return File(file.FileContents, file.ContentType ?? "application/pdf", file.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generate PDF failed.");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
