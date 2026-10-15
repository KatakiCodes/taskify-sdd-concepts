using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFy.Domain.Common;

namespace TaskFy.Api.Middleware;

public static class ProblemDetailsResultExtensions
{
    public static ProblemDetails ToProblemDetails(this Result result, HttpContext? httpContext = null)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot convert a successful result to ProblemDetails.");
        }

        var error = result.Error;
        var (statusCode, type, title) = error.Type switch
        {
            ErrorType.Validation => (
                StatusCodes.Status400BadRequest,
                "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                "Bad Request"),
            ErrorType.NotFound => (
                StatusCodes.Status404NotFound,
                "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                "Not Found"),
            ErrorType.Conflict => (
                StatusCodes.Status409Conflict,
                "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                "Conflict"),
            ErrorType.Unexpected => (
                StatusCodes.Status500InternalServerError,
                "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                "Internal Server Error"),
            _ => (
                StatusCodes.Status400BadRequest,
                "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                "Bad Request")
        };

        if (error.Type == ErrorType.Validation)
        {
            var validationProblem = new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { error.Code, [error.Description] }
            })
            {
                Status = statusCode,
                Type = type,
                Title = title,
                Detail = error.Description,
                Instance = httpContext?.Request.Path
            };

            validationProblem.Extensions["code"] = error.Code;
            return validationProblem;
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Type = type,
            Title = title,
            Detail = error.Description,
            Instance = httpContext?.Request.Path
        };

        problemDetails.Extensions["code"] = error.Code;
        return problemDetails;
    }
}
