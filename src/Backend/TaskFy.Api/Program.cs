using Microsoft.EntityFrameworkCore;
using TaskFy.Api.Extensions;
using TaskFy.Api.Middleware;
using TaskFy.Application;
using TaskFy.Infrastructure;
using TaskFy.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Register application and infrastructure layers
builder.Services.AddApplication(typeof(Program).Assembly);
builder.Services.AddInfrastructure(builder.Configuration);

// Register API services
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDevelopmentCors();
builder.Services.AddScoped<TaskFy.Application.Common.Interfaces.IUserContext, TaskFy.Api.Common.DefaultUserContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskFyDbContext>();
    if (dbContext.Database.IsRelational())
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync();
        }
        else
        {
            await dbContext.Database.EnsureCreatedAsync();
        }
    }
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseStatusCodePages();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseDevelopmentCors();
}

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

public partial class Program
{
}
