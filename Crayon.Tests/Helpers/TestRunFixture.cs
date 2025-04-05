using Crayon.Configuration;

namespace Crayon.Tests.Helpers;

public class TestRunFixture : IAsyncDisposable
{
    public string Id
    {
        get;
    } = "db"+Guid.NewGuid().ToString().Replace("-","");

    private string mainConnectionString;
    private string queueConnectionString;

    public void SetConnectionStrings(ContainerFixture containerFixture)
    {
        mainConnectionString = containerFixture.GetConnectionString("db");
        queueConnectionString = containerFixture.GetConnectionString("queue");
    }
    
    public async ValueTask DisposeAsync()
    {
        
    }

    public AppSettings AdjustApplicationSettings(AppSettings settings)
    {
        settings.ConnectionString = mainConnectionString.Replace("it_db", Id);
        settings.PublisherConfiguration.Url = queueConnectionString;
        return settings;
    }
    
}