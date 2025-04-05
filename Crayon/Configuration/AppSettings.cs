namespace Crayon.Configuration;

public class AppSettings
{
    public int WiremockPort { get; set; } = 7777;
    public string CCPBaseAddress  { get; set; }
    public bool ConfigureJWT { get; set; }
    public string ConnectionString { get; set; } = "";
    public string RedisConnectionString { get; set; } = "";
    public PublisherConfiguration PublisherConfiguration { get; set; }
}

public class PublisherConfiguration
{
    public string Url { get; set; }
}
