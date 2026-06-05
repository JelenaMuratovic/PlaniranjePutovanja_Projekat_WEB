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
        //public static IActionResult MapException(ControllerBase controller, Exception ex)
        //{
        //    var safeException = UnwrapException(ex);

        //    var exceptionTypeName = safeException.GetType().FullName;

        //    if (exceptionTypeName != null && exceptionTypeName.Contains("ValidationException"))
        //    {
        //        // Bilo da je FluentValidation ili DataAnnotations, tretiramo ga kao 400 Bad Request
        //        return controller.BadRequest(new { error = safeException.Message });
        //    }

        //    return safeException switch
        //    {
        //        KeyNotFoundException keyNotFoundEx =>
        //            controller.NotFound(new { error = keyNotFoundEx.Message }),

        //        InvalidOperationException invalidOpEx =>
        //            controller.Conflict(new { error = invalidOpEx.Message }),

        //        UnauthorizedAccessException =>
        //            controller.Forbid(),

        //        _ => controller.StatusCode(500, new { error = "The server could not complete the request right now." })
        //    };
        //}

        //private static Exception UnwrapException(Exception ex)
        //{
        //    if (ex is AggregateException aggregateException)
        //    {
        //        return aggregateException.InnerException ?? ex;
        //    }

        //    if (ex.InnerException != null)
        //    {
        //        return ex.InnerException;
        //    }

        //    return ex;
        //}
        //public static IActionResult MapException(ControllerBase controller, Exception ex)
        //{
        //    var safeException = UnwrapException(ex);

        //    return safeException switch
        //    {
        //        ValidationException => controller.BadRequest(new { error = GetSafeMessage(safeException.Message, "Please check the entered data and try again.") }),
        //        _ when safeException.GetType().Name == "ValidationException" => controller.BadRequest(new { error = GetSafeMessage(safeException.Message, "Please check the entered data and try again.") }),
        //        KeyNotFoundException => controller.NotFound(new { error = "The requested item could not be found." }),
        //        UnauthorizedAccessException => controller.Forbid(),
        //        InvalidOperationException => controller.Conflict(new { error = GetSafeMessage(safeException.Message, "This action cannot be completed with the current data.") }),
        //        _ => controller.StatusCode(500, new { error = "The server could not complete the request right now." })
        //    };
        //}

        //private static Exception UnwrapException(Exception ex)
        //{
        //    return ex is AggregateException { InnerExceptions.Count: 1 } aggregateException
        //        ? aggregateException.InnerExceptions[0]
        //        : ex;
        //}

        //private static string GetSafeMessage(string message, string fallback)
        //{
        //    if (string.IsNullOrWhiteSpace(message))
        //    {
        //        return fallback;
        //    }

        //    var trimmedMessage = UnwrapAggregateMessage(message.Trim());
        //    return HasSensitiveDetails(trimmedMessage) ? fallback : trimmedMessage;
        //}

        //private static string UnwrapAggregateMessage(string message)
        //{
        //    const string prefix = "One or more errors occurred. (";
        //    return message.StartsWith(prefix) && message.EndsWith(")")
        //        ? message[prefix.Length..^1]
        //        : message;
        //}

        //private static bool HasSensitiveDetails(string message)
        //{
        //    var normalizedMessage = message.ToLowerInvariant();
        //    var hasIdLikeValue = Regex.IsMatch(
        //        message,
        //        @"\b[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\b|\b[a-z0-9_-]{16,}\b",
        //        RegexOptions.IgnoreCase);

        //    return normalizedMessage.Contains("with id") ||
        //           normalizedMessage.Contains("userid") ||
        //           normalizedMessage.Contains("travelid") ||
        //           normalizedMessage.Contains("destinationid") ||
        //           normalizedMessage.Contains("expenseid") ||
        //           normalizedMessage.Contains("activityid") ||
        //           normalizedMessage.Contains("token") ||
        //           normalizedMessage.Contains("exception") ||
        //           hasIdLikeValue;
        //}
    }
}
