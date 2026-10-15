using Moq;
using TaskFy.Application.Common.Interfaces;
using TaskFy.Application.Features.Tasks.Queries.GetTaskById;
using TaskFy.Domain.Entities;
using TaskFy.Domain.Errors;
using TaskFy.Domain.Repositories;
using Xunit;

namespace TaskFy.UnitTests.Features.Tasks;

public class GetTaskByIdQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly GetTaskByIdQueryHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public GetTaskByIdQueryHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _userContextMock = new Mock<IUserContext>();

        _userContextMock.Setup(u => u.CurrentUserId).Returns(_userId);

        _handler = new GetTaskByIdQueryHandler(_taskRepositoryMock.Object, _userContextMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WhenTaskExists_ShouldReturnSuccessWithTaskDto()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var taskResult = TaskItem.Create(_userId, "Tarefa Existente", "Descrição da tarefa", DateTimeOffset.UtcNow, null);
        var task = taskResult.Value;

        _taskRepositoryMock
            .Setup(r => r.GetByIdAsync(taskId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        var query = new GetTaskByIdQuery(taskId);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(task.Name, result.Value.Name);
        Assert.Equal(task.Description, result.Value.Description);
        Assert.Equal(_userId, result.Value.UserId);
    }

    [Fact]
    public async Task HandleAsync_WhenTaskDoesNotExist_ShouldReturnFailureWithNotFound()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _taskRepositoryMock
            .Setup(r => r.GetByIdAsync(taskId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        var query = new GetTaskByIdQuery(taskId);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TaskErrors.NotFound.Code, result.Error.Code);
    }
}
