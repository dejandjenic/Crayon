using Crayon.Configuration;
using DotNet.Testcontainers.Containers;
using MySqlConnector;
using Testcontainers.MariaDb;

namespace TestProject1;

public class TestBase
{
    public TestBase()
    {
        InitContainers().GetAwaiter().GetResult();
    }
    
    public AppSettings AdjustSettings(AppSettings settings)
    {
        settings.ConnectionString = GetConnectionString("db");
        return settings;
    }

    private Dictionary<string,DockerContainer> containers = new ();

    protected string GetConnectionString(string container)
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
        var CurrentTestId = Guid.NewGuid().ToString();
        var mariaDbContainerTask = Task.Run(
            () =>
                new MariaDbBuilder()
                    .WithName($"it_{CurrentTestId}_mariadb")
                    //.WithNetworkAliases(nameof(_mariaDbContainer))
                    //.DependsOn(_network)
                    .WithCleanUp(true)
                    .WithDatabase("it_db")
                    .WithUsername("root")
                    .WithPassword("root")
                    //.WithBindMount(mariaDbCnfPath, "/etc/mysql/conf.d")
                    .Build()
        );
        
        var _mariaDbContainer = await mariaDbContainerTask;
        _mariaDbContainer.Started += (_, _) =>
        {
            var mariaDbConnectionString = _mariaDbContainer.GetConnectionString();
            var conn = new MySqlConnection(mariaDbConnectionString);

            // sanity test the connection
            conn.Open();

            var cmd = new MySqlCommand(File.ReadAllText("../../../../Schema.sql"), conn);
            cmd.ExecuteNonQuery();
            
            cmd = new MySqlCommand(File.ReadAllText("../../../../Seed.sql"), conn);
            cmd.ExecuteNonQuery();
            
            conn.Close();
            conn.Dispose();

            // SqlMigrator.ApplyMigrations(mariaDbConnectionString);
            // sqlDbMigrated = true;

            //Logger<>.LogDebug("SQL database migrated");
        };
        await _mariaDbContainer.StartAsync();
        
        containers.Add("db",_mariaDbContainer);
    }
}