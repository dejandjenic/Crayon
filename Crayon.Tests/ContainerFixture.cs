using Crayon.Configuration;
using DotNet.Testcontainers.Containers;
using Testcontainers.MariaDb;

namespace Crayon.Tests;

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<ContainerFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

public class ContainerFixture : IAsyncDisposable
{
    public ContainerFixture()
    {
        InitContainers().GetAwaiter().GetResult();
    }
    

    private Dictionary<string,DockerContainer> containers = new ();

    public string GetConnectionString(string container)
    {
        return container switch
        {
            "db" => (containers[container] as MariaDbContainer).GetConnectionString(),
            _ => string.Empty
        };
    }

    protected async Task StopAllContainers()
    {
        foreach (var it in containers)
        {
            await it.Value.StopAsync();
        }
    }

    async Task InitContainers()
    {
        var mariaDbContainerTask = Task.Run(
            () =>
                new MariaDbBuilder()
                    .WithName($"it_{Guid.NewGuid().ToString()}_mariadb")
                    .WithCleanUp(true)
                    .WithDatabase("it_db")
                    .WithUsername("root")
                    .WithPassword("root")
                    .Build()
        );
        
        var _mariaDbContainer = await mariaDbContainerTask;
        await _mariaDbContainer.StartAsync();
        
        containers.Add("db",_mariaDbContainer);
    }

    public async ValueTask DisposeAsync()
    {
        await StopAllContainers();
    }
}