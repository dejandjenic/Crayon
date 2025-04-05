using Crayon.Configuration;

namespace TestProject1;

public class TestRunFixture : IAsyncDisposable
{
    public string Id
    {
        get;
    } = "db"+Guid.NewGuid().ToString().Replace("-","");

    public string MainConnectionString { get; set; }
    
    public async ValueTask DisposeAsync()
    {
        
    }

    public AppSettings AdjustApplicationSettings(AppSettings settings)
    {
        settings.ConnectionString = MainConnectionString.Replace("it_db", Id);
        return settings;
    }
    
}