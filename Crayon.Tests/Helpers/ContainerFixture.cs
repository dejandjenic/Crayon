using DotNet.Testcontainers.Containers;
using Testcontainers.MariaDb;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Crayon.Tests.Helpers;

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<ContainerFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

public enum ContainerType
{
    Db,
    Queue,
    Cache
}

public class ContainerFixture : IAsyncDisposable
{

    public ContainerFixture()
    {
        InitContainers().GetAwaiter().GetResult();
    }
    

    private Dictionary<ContainerType,DockerContainer> containers = new ();

    public string GetConnectionString(ContainerType container)
    {
        return container switch
        {
            ContainerType.Db => (containers[container] as MariaDbContainer).GetConnectionString(),
            ContainerType.Queue => (containers[container] as RabbitMqContainer).GetConnectionString(),
            ContainerType.Cache => (containers[container] as RedisContainer).GetConnectionString(),
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
        var currentTestId = Guid.NewGuid().ToString();

        var mariaDbContainerTask = StartDatabase(currentTestId);
        var rabbitMqContainerTask = StartQueue(currentTestId);
        var redisContainerTask = StartRedis(currentTestId);

        await Task.WhenAll(mariaDbContainerTask, rabbitMqContainerTask,redisContainerTask);

        var mariaDbContainer = mariaDbContainerTask.Result;
        var rabbitMqContainer = rabbitMqContainerTask.Result;
        var redisContainer = redisContainerTask.Result;
        
        containers.Add(ContainerType.Db,mariaDbContainer);
        containers.Add(ContainerType.Queue,rabbitMqContainer);
        containers.Add(ContainerType.Cache,redisContainer);
    }

    async Task<RedisContainer> StartRedis(string currentTestId)
    {
        var redisCnfPath = Path.GetFullPath("../../../../redis");
        if (!Directory.Exists(redisCnfPath))
        {
            throw new FileNotFoundException("Required redis conf dir not found");
        }
        
        var redisContainer = new RedisBuilder()
            .WithName($"it_{currentTestId}_redis")
            .WithCleanUp(true)
            .WithBindMount(redisCnfPath,"/usr/local/etc/redis")
            //.WithEntrypoint("redis-server /usr/local/etc/redis/redis.conf")
            //.WithCommand("redis-server /usr/local/etc/redis/redis.conf")
            .Build();

        await redisContainer.StartAsync();
        await redisContainer.ExecAsync(new List<string>() { "redis-cli", "config", "set", "notify-keyspace-events", "KEA" });
        return redisContainer;
    }

    async Task<RabbitMqContainer> StartQueue(string currentTestId)
    {
        var rabbitMqContainerTask = Task.Run(
            () =>
                new RabbitMqBuilder()
                    .WithName($"it_{currentTestId}_rabbitmq")
                    .WithCleanUp(true)
                    .WithUsername("guest")
                    .WithPassword("guest")
                    .WithEnvironment("RABBITMQ_DEFAULT_VHOST", "/")
                    .Build()
        );
        
        var rabbitMqContainer = await rabbitMqContainerTask;
        await rabbitMqContainer.StartAsync();
        return rabbitMqContainer;
    }
    async Task<MariaDbContainer> StartDatabase(string currentTestId)
    {
        var mariaDbContainerTask = Task.Run(
            () =>
                new MariaDbBuilder()
                    .WithName($"it_{currentTestId}_mariadb")
                    .WithCleanUp(true)
                    .WithDatabase("it_db")
                    .WithUsername("root")
                    .WithPassword("root")
                    .Build()
        );
        
        var mariaDbContainer = await mariaDbContainerTask;
        await mariaDbContainer.StartAsync();
        return mariaDbContainer;
    }

    public async ValueTask DisposeAsync()
    {
        await StopAllContainers();
    }
}