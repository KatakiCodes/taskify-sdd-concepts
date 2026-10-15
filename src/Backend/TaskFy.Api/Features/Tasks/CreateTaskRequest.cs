namespace TaskFy.Api.Features.Tasks;

public sealed record CreateTaskRequest(
    string Name,
    string? Description,
    DateTimeOffset StartDate,
    DateTimeOffset? EndDate);
