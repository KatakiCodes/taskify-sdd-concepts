using Microsoft.EntityFrameworkCore;
using TaskFy.Domain.Entities;
using TaskFy.Domain.Enums;
using TaskFy.Domain.Repositories;

namespace TaskFy.Infrastructure.Persistence.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly TaskFyDbContext _dbContext;

    public TaskRepository(TaskFyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> SearchAsync(
        Guid userId,
        string? searchTerm,
        TaskItemStatus? status,
        DateTimeOffset? createdFrom,
        DateTimeOffset? createdTo,
        DateTimeOffset? dueFrom,
        DateTimeOffset? dueTo,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";
            query = query.Where(t =>
                EF.Functions.Like(t.Name, term) ||
                (t.Description != null && EF.Functions.Like(t.Description, term)));
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (createdFrom.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= createdFrom.Value);
        }

        if (createdTo.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= createdTo.Value);
        }

        if (dueFrom.HasValue)
        {
            query = query.Where(t => t.EndDate.HasValue && t.EndDate.Value >= dueFrom.Value);
        }

        if (dueTo.HasValue)
        {
            query = query.Where(t => t.EndDate.HasValue && t.EndDate.Value <= dueTo.Value);
        }

        var results = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        return results.AsReadOnly();
    }

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        await _dbContext.Tasks.AddAsync(task, cancellationToken);
    }

    public void Update(TaskItem task)
    {
        _dbContext.Tasks.Update(task);
    }

    public void Delete(TaskItem task)
    {
        _dbContext.Tasks.Remove(task);
    }
}
