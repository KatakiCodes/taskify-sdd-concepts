using TaskFy.Application.Common.Interfaces;
using TaskFy.Application.Cqrs;
using TaskFy.Application.Features.Tasks.DTOs;
using TaskFy.Domain.Common;
using TaskFy.Domain.Entities;
using TaskFy.Domain.Repositories;

namespace TaskFy.Application.Features.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandHandler : ICommandHandler<CreateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public CreateTaskCommandHandler(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<TaskDto>> HandleAsync(CreateTaskCommand command, CancellationToken cancellationToken = default)
    {
        var creationResult = TaskItem.Create(
            _userContext.CurrentUserId,
            command.Name,
            command.Description,
            command.StartDate,
            command.EndDate);

        if (!creationResult.IsSuccess)
        {
            return Result<TaskDto>.Failure(creationResult.Error);
        }

        var task = creationResult.Value;

        await _taskRepository.AddAsync(task, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

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
