using System.Reflection;
using Xunit;

namespace TaskFy.UnitTests.Architecture;

public class FileIsolationTests
{
    [Fact]
    public void DomainAndApplicationTypes_ShouldBePublicAndIsolated()
    {
        var domainAssembly = typeof(TaskFy.Domain.Common.Entity).Assembly;
        var appAssembly = typeof(TaskFy.Application.Cqrs.Dispatcher).Assembly;

        var domainTypes = domainAssembly.GetExportedTypes().Where(t => !t.IsNested).ToList();
        var appTypes = appAssembly.GetExportedTypes().Where(t => !t.IsNested).ToList();

        Assert.NotEmpty(domainTypes);
        Assert.NotEmpty(appTypes);

        foreach (var type in domainTypes.Concat(appTypes))
        {
            Assert.True(type.IsPublic, $"Type {type.Name} must be public.");
        }
    }
}
