using TaskFy.Domain.Entities;
using TaskFy.Domain.Enums;
using TaskFy.Domain.Errors;
using TaskFy.Domain.Events;
using Xunit;

namespace TaskFy.UnitTests.Domain;

public class TaskItemTests
{
    private readonly Guid _userId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidParameters_ShouldReturnSuccessWithPendingStatusAndDomainEvent()
    {
        // Arrange
        var name = "Implementar Autenticação";
        var description = "Adicionar suporte a JWT e Refresh Tokens";
        var startDate = DateTimeOffset.UtcNow;
        var endDate = startDate.AddDays(7);

        // Act
        var result = TaskItem.Create(_userId, name, description, startDate, endDate);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_userId, result.Value.UserId);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(description, result.Value.Description);
        Assert.Equal(TaskItemStatus.Pending, result.Value.Status);
        Assert.Equal(startDate, result.Value.StartDate);
        Assert.Equal(endDate, result.Value.EndDate);
        Assert.NotEqual(Guid.Empty, result.Value.Id);

        var domainEvents = result.Value.DomainEvents;
        Assert.Single(domainEvents);
        var createdEvent = Assert.IsType<TaskCreatedDomainEvent>(domainEvents[0]);
        Assert.Equal(result.Value.Id, createdEvent.TaskId);
        Assert.Equal(_userId, createdEvent.UserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyOrNullName_ShouldReturnFailureInvalidName(string? invalidName)
    {
        // Arrange
        var startDate = DateTimeOffset.UtcNow;

        // Act
        var result = TaskItem.Create(_userId, invalidName!, "Descrição válida", startDate, null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TaskErrors.InvalidName.Code, result.Error.Code);
    }

    [Fact]
    public void Create_WithNameShorterThan3Characters_ShouldReturnFailureInvalidName()
    {
        // Arrange
        var shortName = "AB";
        var startDate = DateTimeOffset.UtcNow;

        // Act
        var result = TaskItem.Create(_userId, shortName, null, startDate, null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TaskErrors.InvalidName.Code, result.Error.Code);
    }

    [Fact]
    public void Create_WithNameLongerThan150Characters_ShouldReturnFailureInvalidName()
    {
        // Arrange
        var longName = new string('A', 151);
        var startDate = DateTimeOffset.UtcNow;

        // Act
        var result = TaskItem.Create(_userId, longName, null, startDate, null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TaskErrors.InvalidName.Code, result.Error.Code);
    }

    [Fact]
    public void Create_WithEndDateBeforeStartDate_ShouldReturnFailureInvalidDates()
    {
        // Arrange
        var startDate = DateTimeOffset.UtcNow;
        var endDate = startDate.AddDays(-1);

        // Act
        var result = TaskItem.Create(_userId, "Tarefa com data inválida", null, startDate, endDate);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TaskErrors.InvalidDates.Code, result.Error.Code);
    }

    [Fact]
    public void Create_WithNullEndDate_ShouldReturnSuccess()
    {
        // Arrange
        var startDate = DateTimeOffset.UtcNow;

        // Act
        var result = TaskItem.Create(_userId, "Tarefa sem data de término", null, startDate, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.EndDate);
    }
}
