using Moq;
using TaskFy.Application.Common.Interfaces;
using TaskFy.Application.Features.Tasks.Commands.CreateTask;
using TaskFy.Domain.Entities;
using TaskFy.Domain.Errors;
using TaskFy.Domain.Repositories;
using Xunit;

namespace TaskFy.UnitTests.Features.Tasks;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly CreateTaskCommandHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public CreateTaskCommandHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();

        _userContextMock.Setup(u => u.CurrentUserId).Returns(_userId);

        _handler = new CreateTaskCommandHandler(
            _taskRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _userContextMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldPersistTaskCommitAndReturnSuccess()
    {
        // Arrange
        var command = new CreateTaskCommand(
            Name: "Implementar API de Tarefas",
            Description: "Desenvolver endpoints com CQRS",
            StartDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddDays(3));

        _taskRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_userId, result.Value.UserId);
        Assert.Equal(command.Name, result.Value.Name);
        Assert.Equal(command.Description, result.Value.Description);
        Assert.Equal("Pending", result.Value.Status);
        Assert.Equal(command.StartDate, result.Value.StartDate);
        Assert.Equal(command.EndDate, result.Value.EndDate);

        _taskRepositoryMock.Verify(
            r => r.AddAsync(It.Is<TaskItem>(t => t.Name == command.Name && t.UserId == _userId), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidCommand_ShouldReturnFailureAndNotPersistOrCommit()
    {
        // Arrange
        var command = new CreateTaskCommand(
            Name: "AB", // Shorter than 3 chars
            Description: null,
            StartDate: DateTimeOffset.UtcNow,
            EndDate: null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TaskErrors.InvalidName.Code, result.Error.Code);

        _taskRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
