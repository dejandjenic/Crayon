using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Crayon.Events.Base;

public interface IPublisher
{
    Task Start();
    Task Publish<T>(T data);
}


public class Publisher<T>(ConnectionFactory factory) : IPublisher
{
    private IConnection connection;
    private IChannel channel;
    
    public virtual string Topic { get; set; }
    public virtual string RoutingKey { get; set; }
    
    public async Task Start()
    {
        connection = await factory.CreateConnectionAsync();
        channel = await connection.CreateChannelAsync();
        
        await channel.ExchangeDeclareAsync(exchange: Topic, type: ExchangeType.Topic);
    }
    
    public async Task Publish<T>(T data)
    {
        var message = JsonSerializer.Serialize(data);
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: Topic, routingKey: RoutingKey, body: body);
    }
}