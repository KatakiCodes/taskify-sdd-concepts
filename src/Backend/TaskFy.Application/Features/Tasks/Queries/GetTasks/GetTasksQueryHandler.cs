using TaskFy.Application.Common.Interfaces;
using TaskFy.Application.Cqrs;
using TaskFy.Application.Features.Tasks.DTOs;
using TaskFy.Domain.Common;
using TaskFy.Domain.Repositories;

namespace TaskFy.Application.Features.Tasks.Queries.GetTasks;

public sealed class GetTasksQueryHandler : IQueryHandler<GetTasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserContext _userContext;

    public GetTasksQueryHandler(ITaskRepository taskRepository, IUserContext userContext)
    {
        _taskRepository = taskRepository;
        _userContext = userContext;
    }

    public async Task<Result<IReadOnlyList<TaskDto>>> HandleAsync(GetTasksQuery query, CancellationToken cancellationToken = default)
    {
        var tasks = await _taskRepository.SearchAsync(
            _userContext.CurrentUserId,
            query.SearchTerm,
            query.Status,
            query.CreatedFrom,
            query.CreatedTo,
            query.DueFrom,
            query.DueTo,
            cancellationToken);

        var dtos = tasks.Select(t => new TaskDto(
            t.Id,
            t.UserId,
            t.Name,
            t.Description,
            t.Status.ToString(),
            t.StartDate,
            t.EndDate,
            t.CreatedAt,
            t.UpdatedAt)).ToList().AsReadOnly();

        return Result<IReadOnlyList<TaskDto>>.Success(dtos);
    }
}
