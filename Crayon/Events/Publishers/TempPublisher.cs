using Crayon.Events.Base;
using Crayon.Events.Handlers;
using RabbitMQ.Client;

namespace Crayon.Events.Publishers;

public interface ITempPublisher : IPublisher
{
    
}

public class TempPublisher(ConnectionFactory factory) : Publisher<TempMessage>(factory), ITempPublisher
{
    public override string Topic { get; set; } = Constants.PurchaseTopic;
    public override string RoutingKey { get; set; } = "purchase";
}