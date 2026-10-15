using TaskFy.Application.Cqrs;
using TaskFy.Application.Features.Tasks.DTOs;
using TaskFy.Domain.Enums;

namespace TaskFy.Application.Features.Tasks.Queries.GetTasks;

public sealed record GetTasksQuery(
    string? SearchTerm,
    TaskItemStatus? Status,
    DateTimeOffset? CreatedFrom,
    DateTimeOffset? CreatedTo,
    DateTimeOffset? DueFrom,
    DateTimeOffset? DueTo) : IQuery<IReadOnlyList<TaskDto>>;
