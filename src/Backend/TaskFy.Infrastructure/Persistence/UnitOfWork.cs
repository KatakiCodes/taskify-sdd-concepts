using TaskFy.Domain.Repositories;

namespace TaskFy.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly TaskFyDbContext _dbContext;

    public UnitOfWork(TaskFyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        _dbContext.ChangeTracker.Clear();
        return Task.CompletedTask;
    }
}
