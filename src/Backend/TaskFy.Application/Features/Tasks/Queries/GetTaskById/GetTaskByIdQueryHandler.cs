using TaskFy.Application.Common.Interfaces;
using TaskFy.Application.Cqrs;
using TaskFy.Application.Features.Tasks.DTOs;
using TaskFy.Domain.Common;
using TaskFy.Domain.Errors;
using TaskFy.Domain.Repositories;

namespace TaskFy.Application.Features.Tasks.Queries.GetTaskById;

public sealed class GetTaskByIdQueryHandler : IQueryHandler<GetTaskByIdQuery, TaskDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserContext _userContext;

    public GetTaskByIdQueryHandler(ITaskRepository taskRepository, IUserContext userContext)
    {
        _taskRepository = taskRepository;
        _userContext = userContext;
    }

    public async Task<Result<TaskDto>> HandleAsync(GetTaskByIdQuery query, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(query.Id, _userContext.CurrentUserId, cancellationToken);

        if (task is null)
        {
            return Result<TaskDto>.Failure(TaskErrors.NotFound);
        }

        var dto = new TaskDto(
            task.Id,
            task.UserId,
            task.Name,
            task.Description,
            task.Status.ToString(),
            task.StartDate,
            task.EndDate,
            task.CreatedAt,
            task.UpdatedAt);

        return Result<TaskDto>.Success(dto);
    }
}
