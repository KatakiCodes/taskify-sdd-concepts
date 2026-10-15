using Microsoft.AspNetCore.Mvc;
using TaskFy.Api.Middleware;
using TaskFy.Application.Cqrs;

namespace TaskFy.Api.Features.Diagnostics;

[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public HealthController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    [ProducesResponseType(typeof(HealthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHealthAsync(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<GetHealthQuery, HealthResponseDto>(
            new GetHealthQuery(),
            cancellationToken);

        if (result.IsFailure)
        {
            var problemDetails = result.ToProblemDetails(HttpContext);
            return StatusCode(problemDetails.Status ?? StatusCodes.Status500InternalServerError, problemDetails);
        }

        return Ok(result.Value);
    }
}
