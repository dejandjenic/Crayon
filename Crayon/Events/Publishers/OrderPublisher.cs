using Crayon.Events.Base;
using Crayon.Events.Handlers;
using RabbitMQ.Client;

namespace Crayon.Events.Publishers;

public class OrderPublisher(ConnectionFactory factory) : Publisher<OrderMessage>(factory)
{
    public override string Topic { get; set; } = Constants.PurchaseTopic;
    public override string RoutingKey { get; set; } = "purchase.created";
    
}