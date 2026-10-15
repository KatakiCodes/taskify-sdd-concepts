using Microsoft.EntityFrameworkCore;

namespace TaskFy.Infrastructure.Persistence;

public class TaskFyDbContext : DbContext
{
    public TaskFyDbContext(DbContextOptions<TaskFyDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskFyDbContext).Assembly);
    }
}
