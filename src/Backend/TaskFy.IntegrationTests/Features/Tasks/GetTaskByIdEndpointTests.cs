using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFy.Api.Features.Tasks;
using TaskFy.Application.Features.Tasks.DTOs;
using TaskFy.Domain.Errors;
using TaskFy.IntegrationTests.Fixtures;
using Xunit;

namespace TaskFy.IntegrationTests.Features.Tasks;

public class GetTaskByIdEndpointTests : IntegrationTestBase
{
    public GetTaskByIdEndpointTests(TaskFyWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetTaskById_HappyPath_WhenTaskExists_ShouldReturn200Ok_WithTaskDto()
    {
        // Arrange: Create a task first
        var createRequest = new CreateTaskRequest(
            Name: "Tarefa Para Consulta Por ID",
            Description: "Detalhes específicos da tarefa",
            StartDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddDays(5));

        var createResponse = await Client.PostAsJsonAsync("/tasks", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>();
        Assert.NotNull(createdTask);

        // Act: Fetch by ID
        var getResponse = await Client.GetAsync($"/tasks/{createdTask.Id}");

        // Assert: HTTP Status Code & Content Type
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getResponse.Content.Headers.ContentType);
        Assert.Equal("application/json", getResponse.Content.Headers.ContentType.MediaType);

        // Assert: Payload matches
        var fetchedTask = await getResponse.Content.ReadFromJsonAsync<TaskDto>();
        Assert.NotNull(fetchedTask);
        Assert.Equal(createdTask.Id, fetchedTask.Id);
        Assert.Equal(createdTask.Name, fetchedTask.Name);
        Assert.Equal(createdTask.Description, fetchedTask.Description);
        Assert.Equal(createdTask.Status, fetchedTask.Status);
        Assert.Equal(createdTask.UserId, fetchedTask.UserId);
    }

    [Fact]
    public async Task GetTaskById_WhenTaskDoesNotExist_ShouldReturn404NotFound_WithProblemDetails()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/tasks/{nonExistentId}");

        // Assert: HTTP Status Code & Content Type
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);

        // Assert: Problem Details
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
        Assert.True(problem.Extensions.ContainsKey("code"));
        Assert.Equal(TaskErrors.NotFound.Code, problem.Extensions["code"]?.ToString());
    }
}
