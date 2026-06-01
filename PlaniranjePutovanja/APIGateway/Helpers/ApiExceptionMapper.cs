using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PlaniranjePutovanja.APIGateway.Helpers
{
    public static class ApiExceptionMapper
    {
        public static IActionResult MapException(ControllerBase controller, Exception ex)
        {
            return ex switch
            {
                ValidationException => controller.BadRequest(new { error = ex.Message }),
                KeyNotFoundException => controller.NotFound(new { error = ex.Message }),
                UnauthorizedAccessException => controller.Forbid(),
                InvalidOperationException => controller.Conflict(new { error = ex.Message }),
                _ => controller.StatusCode(500, new { error = ex.Message })
            };
        }
    }
}
