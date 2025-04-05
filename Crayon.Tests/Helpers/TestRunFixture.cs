using Crayon.Configuration;

namespace Crayon.Tests.Helpers;

public class TestRunFixture : IAsyncDisposable
{
    public string Id { get; } = "db" + Guid.NewGuid().ToString().Replace("-","");

    private string mainConnectionString;
    private string queueConnectionString;
    private string cacheConnectionString;

    public void SetConnectionStrings(ContainerFixture containerFixture)
    {
        mainConnectionString = containerFixture.GetConnectionString(ContainerType.Db);
        queueConnectionString = containerFixture.GetConnectionString(ContainerType.Queue);
        cacheConnectionString = containerFixture.GetConnectionString(ContainerType.Cache);
    }
    
    public async ValueTask DisposeAsync()
    {
        
    }

    public AppSettings AdjustApplicationSettings(AppSettings settings)
    {
        settings.ConnectionString = mainConnectionString.Replace("it_db", Id);
        settings.PublisherConfiguration.Url = queueConnectionString;
        settings.RedisConnectionString = cacheConnectionString;
        return settings;
    }
    
}