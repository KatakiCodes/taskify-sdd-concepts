using Microsoft.Extensions.Hosting;
using TaskFy.Application.Cqrs;
using TaskFy.Domain.Common;

namespace TaskFy.Api.Features.Diagnostics;

public sealed class GetHealthQueryHandler : IQueryHandler<GetHealthQuery, HealthResponseDto>
{
    private readonly IHostEnvironment _environment;

    public GetHealthQueryHandler(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public Task<Result<HealthResponseDto>> HandleAsync(GetHealthQuery query, CancellationToken cancellationToken = default)
    {
        var response = new HealthResponseDto
        {
            Status = "Healthy",
            Timestamp = DateTimeOffset.UtcNow,
            Version = typeof(GetHealthQueryHandler).Assembly.GetName().Version?.ToString() ?? "1.0.0",
            Environment = _environment.EnvironmentName
        };

        return Task.FromResult(Result<HealthResponseDto>.Success(response));
    }
}
