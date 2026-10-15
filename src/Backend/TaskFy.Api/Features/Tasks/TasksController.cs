using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFy.Api.Middleware;
using TaskFy.Application.Cqrs;
using TaskFy.Application.Features.Tasks.Commands.CreateTask;
using TaskFy.Application.Features.Tasks.DTOs;

namespace TaskFy.Api.Features.Tasks;

[ApiController]
[Route("tasks")]
public sealed class TasksController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public TasksController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTaskAsync([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTaskCommand(
            request.Name,
            request.Description,
            request.StartDate,
            request.EndDate);

        var result = await _dispatcher.SendAsync<CreateTaskCommand, TaskDto>(command, cancellationToken);

        if (result.IsFailure)
        {
            var problemDetails = result.ToProblemDetails(HttpContext);
            return StatusCode(problemDetails.Status ?? StatusCodes.Status400BadRequest, problemDetails);
        }

        return Created($"/tasks/{result.Value.Id}", result.Value);
    }
}
