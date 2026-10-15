using TaskFy.Domain.Common;

namespace TaskFy.Domain.Events;

public sealed record TaskCreatedDomainEvent(Guid TaskId, Guid UserId, DateTimeOffset OccurredOn) : IDomainEvent;
