using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFy.Api.Middleware;
using TaskFy.Application.Cqrs;
using TaskFy.Application.Features.Tasks.Commands.CreateTask;
using TaskFy.Application.Features.Tasks.DTOs;

using TaskFy.Application.Features.Tasks.Queries.GetTaskById;

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

    [HttpGet("{id:guid}", Name = "GetTaskById")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTaskByIdQuery(id);
        var result = await _dispatcher.QueryAsync<GetTaskByIdQuery, TaskDto>(query, cancellationToken);

        if (result.IsFailure)
        {
            var problemDetails = result.ToProblemDetails(HttpContext);
            return StatusCode(problemDetails.Status ?? StatusCodes.Status404NotFound, problemDetails);
        }

        return Ok(result.Value);
    }
}
