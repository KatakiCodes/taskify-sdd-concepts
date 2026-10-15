namespace TaskFy.Application.Features.Tasks.DTOs;

public sealed record TaskDto(
    Guid Id,
    Guid UserId,
    string Name,
    string? Description,
    string Status,
    DateTimeOffset StartDate,
    DateTimeOffset? EndDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
