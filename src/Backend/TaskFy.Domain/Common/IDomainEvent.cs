namespace TaskFy.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
