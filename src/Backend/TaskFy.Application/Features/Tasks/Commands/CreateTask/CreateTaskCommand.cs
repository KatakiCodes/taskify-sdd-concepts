using TaskFy.Application.Cqrs;
using TaskFy.Application.Features.Tasks.DTOs;
using TaskFy.Domain.Common;

namespace TaskFy.Application.Features.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    string Name,
    string? Description,
    DateTimeOffset StartDate,
    DateTimeOffset? EndDate) : ICommand<TaskDto>;
