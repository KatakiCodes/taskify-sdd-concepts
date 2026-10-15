using TaskFy.Domain.Entities;
using TaskFy.Domain.Enums;

namespace TaskFy.Domain.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> SearchAsync(
        Guid userId,
        string? searchTerm,
        TaskItemStatus? status,
        DateTimeOffset? createdFrom,
        DateTimeOffset? createdTo,
        DateTimeOffset? dueFrom,
        DateTimeOffset? dueTo,
        CancellationToken cancellationToken = default);

    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);

    void Update(TaskItem task);

    void Delete(TaskItem task);
}
