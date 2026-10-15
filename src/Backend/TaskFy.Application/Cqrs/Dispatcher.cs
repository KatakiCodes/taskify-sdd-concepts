using Microsoft.Extensions.DependencyInjection;
using TaskFy.Application.Common;

namespace TaskFy.Application.Cqrs;

public sealed class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>()
            ?? throw new InvalidOperationException($"Handler for command {typeof(TCommand).Name} is not registered.");

        return await handler.HandleAsync(command, cancellationToken);
    }

    public async Task<Result<TResult>> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
    {
        var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResult>>()
            ?? throw new InvalidOperationException($"Handler for command {typeof(TCommand).Name} returning {typeof(TResult).Name} is not registered.");

        return await handler.HandleAsync(command, cancellationToken);
    }

    public async Task<Result<TResult>> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        var handler = _serviceProvider.GetService<IQueryHandler<TQuery, TResult>>()
            ?? throw new InvalidOperationException($"Handler for query {typeof(TQuery).Name} returning {typeof(TResult).Name} is not registered.");

        return await handler.HandleAsync(query, cancellationToken);
    }
}
