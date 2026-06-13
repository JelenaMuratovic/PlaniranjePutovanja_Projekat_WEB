using Microsoft.AspNetCore.Mvc;
using PlaniranjePutovanja.Common.DTOs.Error;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PlaniranjePutovanja.APIGateway.Helpers
{
    public static class ApiExceptionMapper
    {
        public static IActionResult MapException(ControllerBase controller, Exception ex)
        {
            var safeException = UnwrapException(ex);

            var exceptionTypeName =
               safeException.GetType().FullName;

            // FluentValidation + DataAnnotations
            if (exceptionTypeName != null &&
                exceptionTypeName.Contains("ValidationException"))
            {
                return controller.BadRequest(
                    new ErrorResponseDto
                    {
                        Message = safeException.Message
                    });
            }

            return safeException switch
            {
                KeyNotFoundException keyNotFoundEx =>
                    controller.NotFound(
                        new ErrorResponseDto
                        {
                            Message = keyNotFoundEx.Message
                        }),

                InvalidOperationException invalidOpEx =>
                    controller.Conflict(
                        new ErrorResponseDto
                        {
                            Message = invalidOpEx.Message
                        }),

                UnauthorizedAccessException =>
                    controller.Unauthorized(
                        new ErrorResponseDto
                        {
                            Message = "Unauthorized."
                        }),

                _ =>
                    controller.StatusCode(500,
                        new ErrorResponseDto
                        {
                            Message = "An unexpected error occurred."
                        })
            };
        }

        private static Exception UnwrapException(Exception ex)
        {
            if (ex is AggregateException aggregateException)
            {
                return aggregateException.InnerException ?? ex;
            }

            return ex.InnerException ?? ex;
        }        
    }
}
