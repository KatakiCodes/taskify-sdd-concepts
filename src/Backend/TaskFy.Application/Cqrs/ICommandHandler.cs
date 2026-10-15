using TaskFy.Domain.Common;

namespace TaskFy.Application.Cqrs;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
