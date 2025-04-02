namespace Crayon.Configuration;

public class AppSettings
{
    public string ConnectionString { get; set; } = "";
    public PublisherConfiguration PublisherConfiguration { get; set; }
    public List<SubscriberConfiguration> SubscriberConfiguration { get; set; }
}

public class PublisherConfiguration
{
    public string HostName { get; set; }
}

public class SubscriberConfiguration
{
    public string Topic { get; set; }
    public string Type { get; set; }
}