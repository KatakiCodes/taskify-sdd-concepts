using Moq;
using TaskFy.Application.Common.Interfaces;
using TaskFy.Application.Features.Tasks.Queries.GetTasks;
using TaskFy.Domain.Entities;
using TaskFy.Domain.Enums;
using TaskFy.Domain.Repositories;
using Xunit;

namespace TaskFy.UnitTests.Features.Tasks;

public class GetTasksQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly GetTasksQueryHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public GetTasksQueryHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _userContextMock = new Mock<IUserContext>();

        _userContextMock.Setup(u => u.CurrentUserId).Returns(_userId);

        _handler = new GetTasksQueryHandler(_taskRepositoryMock.Object, _userContextMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WhenTasksExist_ShouldReturnSuccessWithProjectedDtos()
    {
        // Arrange
        var task1 = TaskItem.Create(_userId, "Tarefa 1", "Descrição 1", DateTimeOffset.UtcNow, null).Value;
        var task2 = TaskItem.Create(_userId, "Tarefa 2", "Descrição 2", DateTimeOffset.UtcNow, null).Value;
        var taskList = new List<TaskItem> { task1, task2 };

        _taskRepositoryMock
            .Setup(r => r.SearchAsync(
                _userId,
                It.IsAny<string?>(),
                It.IsAny<TaskItemStatus?>(),
                It.IsAny<DateTimeOffset?>(),
                It.IsAny<DateTimeOffset?>(),
                It.IsAny<DateTimeOffset?>(),
                It.IsAny<DateTimeOffset?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskList);

        var query = new GetTasksQuery("Tarefa", null, null, null, null, null);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("Tarefa 1", result.Value[0].Name);
        Assert.Equal("Tarefa 2", result.Value[1].Name);
    }

    [Fact]
    public async Task HandleAsync_WhenNoTasksExist_ShouldReturnSuccessWithEmptyList()
    {
        // Arrange
        _taskRepositoryMock
            .Setup(r => r.SearchAsync(
                _userId,
                It.IsAny<string?>(),
                It.IsAny<TaskItemStatus?>(),
                It.IsAny<DateTimeOffset?>(),
                It.IsAny<DateTimeOffset?>(),
                It.IsAny<DateTimeOffset?>(),
                It.IsAny<DateTimeOffset?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem>());

        var query = new GetTasksQuery(null, null, null, null, null, null);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }
}
