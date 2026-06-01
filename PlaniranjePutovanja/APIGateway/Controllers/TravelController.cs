using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using PlaniranjePutovanja.APIGateway.Helpers;
using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.Common.Interfaces.Travel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PlaniranjePutovanja.APIGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TravelController : ControllerBase
    {
        private readonly ITravelService _travelService;
        private readonly ILogger<TravelController> _logger;

        public TravelController(ITravelService travelService, ILogger<TravelController> logger)
        {
            _travelService = travelService;
            _logger = logger;
        }

        // Endpoints putovanja
        [HttpPost("travels")]
        public async Task<IActionResult> CreateTravel([FromBody] CreateTravelDto request)
        {
            try
            { 
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized("Missing userId in token.");

                var result = await _travelService.CreateTravelAsync(userId, request);
                return Created($"api/travel/travels/{result.Id}", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create travel failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpGet("travels/{id}")]
        [Authorize(Policy = "CanViewTravel")]
        public async Task<IActionResult> GetTravelById(string id)
        {
            try
            {
                var result = await _travelService.GetTravelByIdAsync(id);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get travel by id failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpGet("travels/user/{userId}")]
        [Authorize(Policy = "CanViewTravel")]
        public async Task<IActionResult> GetTravelsByUserId(string userId)
        {
            try
            {
                var result = await _travelService.GetTravelsByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get travels by user id failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpGet("travels")]
        public async Task<IActionResult> GetAllTravels()
        {
            try
            {
                var result = await _travelService.GetAllTravelsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get all travels failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpDelete("travels/{id}")]
        [Authorize(Policy = "CanEditTravel")]
        public async Task<IActionResult> DeleteTravel(string id)
        {
            try
            {
                var result = await _travelService.DeleteTravelAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete travel failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpPut("travels/{id}")]
        [Authorize(Policy = "CanEditTravel")] 
        public async Task<IActionResult> UpdateTravel(string id, [FromBody] UpdateTravelDto request)
        {
            try
            {
                var result = await _travelService.UpdateTravelAsync(id, request);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update travel failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        // Endpoints destinacije
        [HttpPost("travels/{travelId}/destinations")]
        [Authorize(Policy = "CanEditTravel")]
        public async Task<IActionResult> AddDestination(string travelId, [FromBody] CreateDestinationDto request)
        {
            try
            {
                var result = await _travelService.AddDestinationAsync(travelId, request);
                return Created($"api/travel/travels/{travelId}/destinations/{result.Id}", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add destination failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpGet("travels/{travelId}/destinations/{id}")]
        [Authorize(Policy = "CanViewTravel")]
        public async Task<IActionResult> GetDestinationById(string travelId, string id)
        {
            try
            {
                var result = await _travelService.GetDestinationByIdAsync(travelId, id);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get destination by id failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpGet("travels/{travelId}/destinations")]
        [Authorize(Policy = "CanViewTravel")]
        public async Task<IActionResult> GetDestinationsByTravelId(string travelId)
        {
            try
            {
                var result = await _travelService.GetDestinationsByTravelIdAsync(travelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get destinations by travel id failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpDelete("travels/{travelId}/destinations/{id}")]
        [Authorize(Policy = "CanEditTravel")]
        public async Task<IActionResult> DeleteDestination(string travelId, string id)
        {
            try
            {
                var result = await _travelService.DeleteDestinationAsync(travelId, id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete destination failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpPut("travels/{travelId}/destinations/{id}")]
        [Authorize(Policy = "CanEditTravel")]
        public async Task<IActionResult> UpdateDestination(string travelId, string id, [FromBody] UpdateDestinationDto request)
        {
            try
            {
                var result = await _travelService.UpdateDestinationAsync(travelId, id, request);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update destination failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        // Endpoints aktivnosti
        [HttpPost("travels/{travelId}/destinations/{destinationId}/activities")]
        [Authorize(Policy = "CanEditTravel")]
        public async Task<IActionResult> AddActivity(string travelId, string destinationId, [FromBody] CreateActivityDto request)
        {
            try
            {
                var result = await _travelService.AddActivityAsync(travelId, destinationId, request);
                return Created($"api/travel/travels/{travelId}/destinations/{destinationId}/activities/{result.Id}", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add activity failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpGet("travels/{travelId}/destinations/{destinationId}/activities/{id}")]
        [Authorize(Policy = "CanViewTravel")]
        public async Task<IActionResult> GetActivityById(string travelId, string destinationId, string id)
        {
            try
            {
                var result = await _travelService.GetActivityByIdAsync(travelId, destinationId, id);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get activity by id failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpGet("travels/{travelId}/destinations/{destinationId}/activities")]
        [Authorize(Policy = "CanViewTravel")]
        public async Task<IActionResult> GetActivitiesByDestinationId(string travelId, string destinationId)
        {
            try
            {
                var result = await _travelService.GetActivitiesByDestinationIdAsync(travelId, destinationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get activities by destination id failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpDelete("travels/{travelId}/destinations/{destinationId}/activities/{id}")]
        [Authorize(Policy = "CanEditTravel")]
        public async Task<IActionResult> DeleteActivity(string travelId, string destinationId, string id)
        {
            try
            {
                var result = await _travelService.DeleteActivityAsync(travelId, destinationId, id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete activity failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpPut("travels/{travelId}/destinations/{destinationId}/activities/{id}")]
        [Authorize(Policy = "CanEditTravel")]
        public async Task<IActionResult> UpdateActivity(string travelId, string destinationId, string id, [FromBody] UpdateActivityDto request)
        {
            try
            {
                var result = await _travelService.UpdateActivityAsync(travelId, destinationId, id, request);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update activity failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        // Endpoints checklists
        [HttpPost("travels/{travelId}/checklists")]
        [Authorize(Policy = "CanEditTravel")]
        public async Task<IActionResult> AddChecklist(string travelId, [FromBody] CreateChecklistDto request)
        {
            try
            {
                var result = await _travelService.AddChecklistAsync(travelId, request);
                return Created($"api/travel/travels/{travelId}/checklists/{result.Id}", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add checklist failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpGet("travels/{travelId}/checklists/{id}")]
        [Authorize(Policy = "CanViewTravel")]
        public async Task<IActionResult> GetChecklistById(string travelId, string id)
        {
            try
            {
                var result = await _travelService.GetChecklistByIdAsync(travelId, id);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get checklist by id failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpGet("travels/{travelId}/checklists")]
        [Authorize(Policy = "CanViewTravel")]
        public async Task<IActionResult> GetChecklistsByTravelId(string travelId)
        {
            try
            {
                var result = await _travelService.GetChecklistsByTravelIdAsync(travelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get checklists by travel id failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpPut("travels/{travelId}/checklists/{id}/toggle")]
        [Authorize(Policy = "CanEditTravel")]
        public async Task<IActionResult> ToggleChecklist(string travelId, string id, [FromQuery] bool isCompleted)
        {
            try
            {
                var result = await _travelService.ToggleChecklistAsync(travelId, id, isCompleted);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Toggle checklist failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }

        [HttpDelete("travels/{travelId}/checklists/{id}")]
        [Authorize(Policy = "CanEditTravel")]
        public async Task<IActionResult> DeleteChecklist(string travelId, string id)
        {
            try
            {
                var result = await _travelService.DeleteChecklistAsync(travelId, id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete checklist failed.");
                return ApiExceptionMapper.MapException(this, ex);
            }
        }
    }
}
