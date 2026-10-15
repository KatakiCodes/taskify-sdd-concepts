using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TaskFy.IntegrationTests.Fixtures;
using Xunit;

namespace TaskFy.IntegrationTests.Middleware;

public class ProblemDetailsIntegrationTests : IntegrationTestBase
{
    public ProblemDetailsIntegrationTests(TaskFyWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetNonExistentRoute_ShouldReturn404_WithProblemDetails()
    {
        var response = await Client.GetAsync("/api/non-existent-endpoint-route");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Contains("problem+json", response.Content.Headers.ContentType.ToString());

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal(404, problemDetails.Status);
    }
}
