using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TaskFy.IntegrationTests.Fixtures;
using Xunit;

namespace TaskFy.IntegrationTests.Features.Cors;

public class CorsEndpointTests : IntegrationTestBase
{
    public CorsEndpointTests(TaskFyWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetHealth_WithOriginHeader_ShouldIncludeAccessControlAllowOriginHeader()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "http://localhost:4200");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"), "Expected Access-Control-Allow-Origin header");
        Assert.Equal("*", response.Headers.GetValues("Access-Control-Allow-Origin").First());
    }

    [Fact]
    public async Task PreflightOptions_ShouldReturnSuccess_WithCorsHeaders()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Options, "/health");
        request.Headers.Add("Origin", "http://localhost:4200");
        request.Headers.Add("Access-Control-Request-Method", "GET");
        request.Headers.Add("Access-Control-Request-Headers", "Content-Type,Authorization");

        // Act
        var response = await Client.SendAsync(request);

        // Assert: HTTP 200 or 204
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent,
            $"Expected 200 OK or 204 NoContent but received {response.StatusCode}");

        // Assert: Access-Control-Allow-Origin
        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"), "Expected Access-Control-Allow-Origin header");
        Assert.Equal("*", response.Headers.GetValues("Access-Control-Allow-Origin").First());
    }
}
