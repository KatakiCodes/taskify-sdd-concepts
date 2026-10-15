using TaskFy.Application.Common;

namespace TaskFy.Application.Cqrs;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
