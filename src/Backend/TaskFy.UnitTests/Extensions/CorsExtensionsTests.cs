using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TaskFy.Api.Extensions;
using Xunit;

namespace TaskFy.UnitTests.Extensions;

public class CorsExtensionsTests
{
    [Fact]
    public void AddDevelopmentCors_ShouldRegisterCorsServicesAndConfigureDevelopmentPolicy()
    {
        var services = new ServiceCollection();

        services.AddDevelopmentCors();

        var serviceProvider = services.BuildServiceProvider();
        var corsOptions = serviceProvider.GetService<IOptions<CorsOptions>>();

        Assert.NotNull(corsOptions);
        var policy = corsOptions.Value.GetPolicy(CorsExtensions.DefaultDevelopmentPolicyName);
        Assert.NotNull(policy);
        Assert.True(policy.AllowAnyOrigin);
        Assert.True(policy.AllowAnyMethod);
        Assert.True(policy.AllowAnyHeader);
        Assert.False(policy.SupportsCredentials);
    }
}
