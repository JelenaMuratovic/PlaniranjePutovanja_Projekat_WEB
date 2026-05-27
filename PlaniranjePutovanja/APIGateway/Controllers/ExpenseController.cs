using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using PlaniranjePutovanja.Common.DTOs.Expense;
using PlaniranjePutovanja.Common.Interfaces.Expense;

namespace PlaniranjePutovanja.APIGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(IExpenseService expenseService, ILogger<ExpenseController> logger)
        {
            _expenseService = expenseService;
            _logger = logger;
        }


        [HttpPost("travels/{travelId}/expenses")]
        [Authorize(Policy = "CanEditTravel")]
        public async Task<IActionResult> AddExpense(string travelId, [FromBody] CreateExpenseDto request)
        {
            try
            {
                var result = await _expenseService.AddExpenseAsync(travelId, request);
                return Created($"api/travels/{travelId}/expenses/{result.Id}", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add expense failed.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("travels/{travelId}/expenses")]
        [Authorize(Policy = "CanViewTravel")]
        public async Task<IActionResult> GetExpensesByTravelId(string travelId)
        {
            try
            {
                var result = await _expenseService.GetExpensesByTravelIdAsync(travelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get expenses by travel id failed.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("travels/{travelId}/budget")]
        [Authorize(Policy = "CanViewTravel")]
        public async Task<IActionResult> GetBudgetSummary(string travelId)
        {
            try
            {
                var result = await _expenseService.GetBudgetSummaryAsync(travelId);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get budget summary failed.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("travels/{travelId}/expenses/{expenseId}")]
        [Authorize(Policy = "CanEditTravel")]
        public async Task<IActionResult> DeleteExpense(string travelId, string expenseId)
        {
            try
            {
                var result = await _expenseService.DeleteExpenseAsync(travelId, expenseId);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete expense failed.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
