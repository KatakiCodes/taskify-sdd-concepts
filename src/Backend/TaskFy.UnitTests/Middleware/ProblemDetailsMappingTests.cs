using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFy.Api.Middleware;
using TaskFy.Application.Common;
using TaskFy.Domain.Common;
using Xunit;

namespace TaskFy.UnitTests.Middleware;

public class ProblemDetailsMappingTests
{
    [Fact]
    public void ToProblemDetails_Validation_ShouldReturnValidationProblemDetails()
    {
        var error = Error.Validation("Email.Invalid", "The provided email is not valid.");
        var result = Result.Failure(error);

        var problemDetails = result.ToProblemDetails();

        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.Equal("Bad Request", problemDetails.Title);
        Assert.IsType<ValidationProblemDetails>(problemDetails);
        var validationProblem = (ValidationProblemDetails)problemDetails;
        Assert.True(validationProblem.Errors.ContainsKey("Email.Invalid"));
        Assert.Equal("Email.Invalid", problemDetails.Extensions["code"]);
    }

    [Fact]
    public void ToProblemDetails_NotFound_ShouldReturn404()
    {
        var error = Error.NotFound("Task.NotFound", "The task was not found.");
        var result = Result.Failure(error);

        var problemDetails = result.ToProblemDetails();

        Assert.Equal(StatusCodes.Status404NotFound, problemDetails.Status);
        Assert.Equal("Not Found", problemDetails.Title);
        Assert.Equal("Task.NotFound", problemDetails.Extensions["code"]);
    }

    [Fact]
    public void ToProblemDetails_Conflict_ShouldReturn409()
    {
        var error = Error.Conflict("Task.AlreadyCompleted", "The task is already completed.");
        var result = Result.Failure(error);

        var problemDetails = result.ToProblemDetails();

        Assert.Equal(StatusCodes.Status409Conflict, problemDetails.Status);
        Assert.Equal("Conflict", problemDetails.Title);
        Assert.Equal("Task.AlreadyCompleted", problemDetails.Extensions["code"]);
    }

    [Fact]
    public void ToProblemDetails_Unexpected_ShouldReturn500()
    {
        var error = Error.Unexpected("System.Failure", "Unexpected server fault.");
        var result = Result.Failure(error);

        var problemDetails = result.ToProblemDetails();

        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.Equal("Internal Server Error", problemDetails.Title);
        Assert.Equal("System.Failure", problemDetails.Extensions["code"]);
    }
}
