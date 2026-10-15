using System.Linq;
using Microsoft.Extensions.Configuration.UserSecrets;
using Xunit;

namespace TaskFy.UnitTests.Configuration;

public class UserSecretsConfigurationTests
{
    [Fact]
    public void ApiAssembly_ShouldDefineUserSecretsIdAttribute()
    {
        var apiAssembly = typeof(TaskFy.Api.Extensions.CorsExtensions).Assembly;
        var attribute = apiAssembly
            .GetCustomAttributes(typeof(UserSecretsIdAttribute), false)
            .Cast<UserSecretsIdAttribute>()
            .FirstOrDefault();

        Assert.NotNull(attribute);
        Assert.False(string.IsNullOrWhiteSpace(attribute.UserSecretsId));
    }
}
