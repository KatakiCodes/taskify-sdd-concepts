using Moq;
using TaskFy.Application.Common;
using TaskFy.Application.Cqrs;
using Xunit;

namespace TaskFy.UnitTests.Cqrs;

public class DispatcherTests
{
    public sealed record SampleCommand : ICommand;

    public sealed record SampleQuery : IQuery<string>;

    [Fact]
    public async Task SendAsync_Command_ShouldResolveAndExecuteHandler()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockHandler = new Mock<ICommandHandler<SampleCommand>>();

        mockHandler
            .Setup(h => h.HandleAsync(It.IsAny<SampleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        mockServiceProvider
            .Setup(sp => sp.GetService(typeof(ICommandHandler<SampleCommand>)))
            .Returns(mockHandler.Object);

        var dispatcher = new Dispatcher(mockServiceProvider.Object);
        var command = new SampleCommand();

        var result = await dispatcher.SendAsync(command);

        Assert.True(result.IsSuccess);
        mockHandler.Verify(h => h.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task QueryAsync_ShouldResolveAndExecuteHandler()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockHandler = new Mock<IQueryHandler<SampleQuery, string>>();

        mockHandler
            .Setup(h => h.HandleAsync(It.IsAny<SampleQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success("Query Result"));

        mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IQueryHandler<SampleQuery, string>)))
            .Returns(mockHandler.Object);

        var dispatcher = new Dispatcher(mockServiceProvider.Object);
        var query = new SampleQuery();

        var result = await dispatcher.QueryAsync<SampleQuery, string>(query);

        Assert.True(result.IsSuccess);
        Assert.Equal("Query Result", result.Value);
        mockHandler.Verify(h => h.HandleAsync(query, It.IsAny<CancellationToken>()), Times.Once);
    }
}
