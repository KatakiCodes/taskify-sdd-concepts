using TaskFy.Domain.Enums;

namespace TaskFy.Api.Features.Tasks;

public sealed record GetTasksRequest(
    string? SearchTerm,
    TaskItemStatus? Status,
    DateTimeOffset? CreatedFrom,
    DateTimeOffset? CreatedTo,
    DateTimeOffset? DueFrom,
    DateTimeOffset? DueTo);
