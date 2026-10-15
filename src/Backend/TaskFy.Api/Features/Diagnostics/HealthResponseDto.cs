namespace TaskFy.Api.Features.Diagnostics;

public sealed record HealthResponseDto
{
    public string Status { get; init; } = "Healthy";
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public string Version { get; init; } = "1.0.0";
    public string Environment { get; init; } = "Development";
}
