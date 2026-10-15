using Microsoft.EntityFrameworkCore;

namespace TaskFy.Infrastructure.Persistence;

public class TaskFyDbContext : DbContext
{
    public TaskFyDbContext(DbContextOptions<TaskFyDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskFy.Domain.Entities.TaskItem> Tasks => Set<TaskFy.Domain.Entities.TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskFyDbContext).Assembly);
    }
}
