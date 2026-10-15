using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskFy.Api.Features.Tasks;
using TaskFy.Application.Features.Tasks.DTOs;
using TaskFy.Domain.Entities;
using TaskFy.Domain.Errors;
using TaskFy.IntegrationTests.Fixtures;
using Xunit;

namespace TaskFy.IntegrationTests.Features.Tasks;

public class CreateTaskEndpointTests : IntegrationTestBase
{
    public CreateTaskEndpointTests(TaskFyWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task PostTasks_HappyPath_ShouldReturn201Created_WithLocationHeaderAndPersistTask()
    {
        // Arrange
        var request = new CreateTaskRequest(
            Name: "Criar Arquitetura do Sistema",
            Description: "Definir vertical slices e contratos de API",
            StartDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddDays(7));

        // Act
        var response = await Client.PostAsJsonAsync("/tasks", request);

        // Assert: HTTP Status Code & Content Type
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

        // Assert: Payload contract
        var payload = await response.Content.ReadFromJsonAsync<TaskDto>();
        Assert.NotNull(payload);
        Assert.NotEqual(Guid.Empty, payload.Id);
        Assert.Equal(request.Name, payload.Name);
        Assert.Equal(request.Description, payload.Description);
        Assert.Equal("Pending", payload.Status);
        Assert.Equal(Guid.Parse("00000000-0000-0000-0000-000000000001"), payload.UserId);

        // Assert: Location header
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/tasks/{payload.Id}", response.Headers.Location.ToString());

        // Assert: Database persistence
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskFy.Infrastructure.Persistence.TaskFyDbContext>();
        var persisted = await db.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == payload.Id);
        Assert.NotNull(persisted);
        Assert.Equal(request.Name, persisted.Name);
    }

    [Fact]
    public async Task PostTasks_WithInvalidName_ShouldReturn400BadRequest_WithProblemDetails()
    {
        // Arrange: Name with less than 3 characters
        var request = new CreateTaskRequest(
            Name: "AB",
            Description: "Descrição",
            StartDate: DateTimeOffset.UtcNow,
            EndDate: null);

        // Act
        var response = await Client.PostAsJsonAsync("/tasks", request);

        // Assert: HTTP Status Code & Content Type
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);

        // Assert: Validation Problem Details
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.True(problem.Errors.ContainsKey(TaskErrors.InvalidName.Code));
    }

    [Fact]
    public async Task PostTasks_WithEndDateBeforeStartDate_ShouldReturn400BadRequest_WithProblemDetails()
    {
        // Arrange: EndDate earlier than StartDate
        var request = new CreateTaskRequest(
            Name: "Tarefa com Datas Inválidas",
            Description: null,
            StartDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddDays(-2));

        // Act
        var response = await Client.PostAsJsonAsync("/tasks", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.True(problem.Errors.ContainsKey(TaskErrors.InvalidDates.Code));
    }
}
