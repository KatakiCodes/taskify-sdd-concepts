using System.Net;
using System.Net.Http.Json;
using TaskFy.Api.Features.Diagnostics;
using TaskFy.IntegrationTests.Fixtures;
using Xunit;

namespace TaskFy.IntegrationTests.Features.Diagnostics;

public class HealthEndpointTests : IntegrationTestBase
{
    public HealthEndpointTests(TaskFyWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetHealth_HappyPath_ShouldReturn200Ok_WithDiagnosticPayload()
    {
        // Act
        var response = await Client.GetAsync("/health");

        // Assert: HTTP Status Code
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert: Content-Type
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

        // Assert: Response Payload contract
        var payload = await response.Content.ReadFromJsonAsync<HealthResponseDto>();
        Assert.NotNull(payload);
        Assert.Equal("Healthy", payload.Status);
        Assert.False(string.IsNullOrWhiteSpace(payload.Version));
        Assert.False(string.IsNullOrWhiteSpace(payload.Environment));
        Assert.True(payload.Timestamp <= DateTimeOffset.UtcNow);
    }
}
