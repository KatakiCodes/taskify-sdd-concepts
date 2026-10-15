using TaskFy.Application.Cqrs;

namespace TaskFy.Api.Features.Diagnostics;

public sealed record GetHealthQuery : IQuery<HealthResponseDto>;
