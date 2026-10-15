using Xunit;

namespace TaskFy.IntegrationTests.Fixtures;

public abstract class IntegrationTestBase : IClassFixture<TaskFyWebApplicationFactory>
{
    protected TaskFyWebApplicationFactory Factory { get; }
    protected HttpClient Client { get; }

    protected IntegrationTestBase(TaskFyWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }
}
