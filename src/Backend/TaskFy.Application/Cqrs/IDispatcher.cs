using TaskFy.Application.Common;

namespace TaskFy.Application.Cqrs;

public interface IDispatcher
{
    Task<Result> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    Task<Result<TResult>> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>;

    Task<Result<TResult>> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>;
}
