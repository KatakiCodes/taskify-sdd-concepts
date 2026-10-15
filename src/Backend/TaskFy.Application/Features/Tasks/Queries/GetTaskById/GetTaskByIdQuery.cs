using TaskFy.Application.Cqrs;
using TaskFy.Application.Features.Tasks.DTOs;

namespace TaskFy.Application.Features.Tasks.Queries.GetTaskById;

public sealed record GetTaskByIdQuery(Guid Id) : IQuery<TaskDto>;
