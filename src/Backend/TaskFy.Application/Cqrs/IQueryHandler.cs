using TaskFy.Application.Common;

namespace TaskFy.Application.Cqrs;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
