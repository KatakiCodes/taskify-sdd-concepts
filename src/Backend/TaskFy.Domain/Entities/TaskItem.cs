using TaskFy.Domain.Common;
using TaskFy.Domain.Enums;
using TaskFy.Domain.Errors;
using TaskFy.Domain.Events;

namespace TaskFy.Domain.Entities;

public sealed class TaskItem : Entity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskItemStatus Status { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset? EndDate { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private TaskItem() : base()
    {
    }

    private TaskItem(
        Guid id,
        Guid userId,
        string name,
        string? description,
        TaskItemStatus status,
        DateTimeOffset startDate,
        DateTimeOffset? endDate,
        DateTimeOffset createdAt)
        : base(id)
    {
        UserId = userId;
        Name = name;
        Description = description;
        Status = status;
        StartDate = startDate;
        EndDate = endDate;
        CreatedAt = createdAt;
    }

    public static Result<TaskItem> Create(
        Guid userId,
        string name,
        string? description,
        DateTimeOffset startDate,
        DateTimeOffset? endDate)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 3 || name.Trim().Length > 150)
        {
            return Result<TaskItem>.Failure(TaskErrors.InvalidName);
        }

        if (endDate.HasValue && endDate.Value < startDate)
        {
            return Result<TaskItem>.Failure(TaskErrors.InvalidDates);
        }

        var now = DateTimeOffset.UtcNow;
        var task = new TaskItem(
            Guid.NewGuid(),
            userId,
            name.Trim(),
            description?.Trim(),
            TaskItemStatus.Pending,
            startDate,
            endDate,
            now);

        task.AddDomainEvent(new TaskCreatedDomainEvent(task.Id, task.UserId, now));

        return Result<TaskItem>.Success(task);
    }
}
