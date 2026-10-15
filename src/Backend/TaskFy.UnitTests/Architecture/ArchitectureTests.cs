using System.Reflection;
using Xunit;

namespace TaskFy.UnitTests.Architecture;

public class ArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotReference_OtherApplicationLayers()
    {
        var domainAssembly = typeof(TaskFy.Domain.Common.Entity).Assembly;
        var referencedAssemblies = domainAssembly.GetReferencedAssemblies().Select(a => a.Name);

        Assert.DoesNotContain("TaskFy.Application", referencedAssemblies);
        Assert.DoesNotContain("TaskFy.Infrastructure", referencedAssemblies);
        Assert.DoesNotContain("TaskFy.Api", referencedAssemblies);
    }

    [Fact]
    public void Application_ShouldNotReference_InfrastructureOrApiLayers()
    {
        var appAssembly = typeof(TaskFy.Application.Common.Result).Assembly;
        var referencedAssemblies = appAssembly.GetReferencedAssemblies().Select(a => a.Name);

        Assert.DoesNotContain("TaskFy.Infrastructure", referencedAssemblies);
        Assert.DoesNotContain("TaskFy.Api", referencedAssemblies);
    }
}
