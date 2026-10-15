using TaskFy.Domain.Common;
using Xunit;

namespace TaskFy.UnitTests.Common;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        var error = Error.Validation("Test.Code", "Validation failed.");
        var result = Result.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void GenericResult_Success_ShouldContainValue()
    {
        var result = Result<string>.Success("Sample Data");

        Assert.True(result.IsSuccess);
        Assert.Equal("Sample Data", result.Value);
    }

    [Fact]
    public void GenericResult_Failure_ShouldThrowOnValueAccess()
    {
        var error = Error.NotFound("Item.NotFound", "Item not found.");
        var result = Result<string>.Failure(error);

        Assert.True(result.IsFailure);
        Assert.Throws<InvalidOperationException>(() => result.Value);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void GenericResult_ImplicitConversion_ShouldWorkForValueAndError()
    {
        Result<int> successResult = 42;
        Assert.True(successResult.IsSuccess);
        Assert.Equal(42, successResult.Value);

        Result<int> failureResult = Error.Failure("Calc.Error", "Calculation failed.");
        Assert.True(failureResult.IsFailure);
        Assert.Equal("Calc.Error", failureResult.Error.Code);
    }
}
