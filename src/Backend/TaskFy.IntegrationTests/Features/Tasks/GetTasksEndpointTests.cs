using System.Net;
using System.Net.Http.Json;
using TaskFy.Api.Features.Tasks;
using TaskFy.Application.Features.Tasks.DTOs;
using TaskFy.IntegrationTests.Fixtures;
using Xunit;

namespace TaskFy.IntegrationTests.Features.Tasks;

public class GetTasksEndpointTests : IntegrationTestBase
{
    public GetTasksEndpointTests(TaskFyWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetTasks_HappyPath_WithoutFilters_ShouldReturn200Ok_WithListOfTasks()
    {
        // Arrange: Create a task
        var createRequest = new CreateTaskRequest(
            Name: "Tarefa Geral Para Listagem",
            Description: "Descrição para teste de listagem",
            StartDate: DateTimeOffset.UtcNow,
            EndDate: null);

        var createResponse = await Client.PostAsJsonAsync("/tasks", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        // Act: Get all tasks
        var response = await Client.GetAsync("/tasks");

        // Assert: HTTP Status Code & Content Type
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

        // Assert: Payload contract
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
        Assert.NotNull(tasks);
        Assert.NotEmpty(tasks);
        Assert.Contains(tasks, t => t.Name == createRequest.Name);
    }

    [Fact]
    public async Task GetTasks_WithSearchTermFilter_ShouldReturnOnlyMatchingTasks()
    {
        // Arrange
        var uniqueSearchTerm = $"SearchTerm_{Guid.NewGuid():N}";
        var matchedRequest = new CreateTaskRequest(
            Name: $"Tarefa com {uniqueSearchTerm} no titulo",
            Description: "Outro texto",
            StartDate: DateTimeOffset.UtcNow,
            EndDate: null);

        var unmatchedRequest = new CreateTaskRequest(
            Name: "Tarefa Comum",
            Description: "Sem a palavra chave",
            StartDate: DateTimeOffset.UtcNow,
            EndDate: null);

        await Client.PostAsJsonAsync("/tasks", matchedRequest);
        await Client.PostAsJsonAsync("/tasks", unmatchedRequest);

        // Act
        var response = await Client.GetAsync($"/tasks?searchTerm={uniqueSearchTerm}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
        Assert.NotNull(tasks);
        Assert.Single(tasks);
        Assert.Equal(matchedRequest.Name, tasks[0].Name);
    }

    [Fact]
    public async Task GetTasks_WithStatusFilter_ShouldReturnOnlyTasksWithStatus()
    {
        // Arrange
        var request = new CreateTaskRequest(
            Name: "Tarefa com Status Pending",
            Description: null,
            StartDate: DateTimeOffset.UtcNow,
            EndDate: null);

        await Client.PostAsJsonAsync("/tasks", request);

        // Act
        var response = await Client.GetAsync("/tasks?status=Pending");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
        Assert.NotNull(tasks);
        Assert.All(tasks, t => Assert.Equal("Pending", t.Status));
    }
}
