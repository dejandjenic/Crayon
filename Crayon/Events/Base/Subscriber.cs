using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Crayon.Events.Base;



public interface ISubscriber
{
    Task Start();
}

public class Subscriber<T>(ConnectionFactory factory) : ISubscriber
{
    private IConnection connection;
    private IChannel channel;

    public virtual string Topic { get; set; }
    public virtual string BindingKey { get; set; }

    public virtual async Task<bool> HandleData(T data)
    {
        return false;
    }
    
    public async Task Start()
    {
        connection = await factory.CreateConnectionAsync();
        channel = await connection.CreateChannelAsync();
        
        await channel.ExchangeDeclareAsync(exchange: Topic, type: ExchangeType.Topic);
        
        QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
        string queueName = queueDeclareResult.QueueName;
        
        await channel.QueueBindAsync(queue: queueName, exchange: Topic, routingKey: BindingKey);
        
        
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            T data = JsonSerializer.Deserialize<T>(message);
            //var routingKey = ea.RoutingKey;
            //Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
            var handled = await HandleData(data);
            if(handled)
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            else
                await channel.BasicNackAsync(deliveryTag:ea.DeliveryTag, multiple:false, requeue:false);
        };

        await channel.BasicConsumeAsync(queueName, autoAck: false, consumer: consumer);
    }
}