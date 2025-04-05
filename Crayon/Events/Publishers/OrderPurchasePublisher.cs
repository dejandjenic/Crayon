using Crayon.Events.Base;
using Crayon.Events.Handlers;
using RabbitMQ.Client;

namespace Crayon.Events.Publishers;

public class OrderPurchasePublisher(ConnectionFactory factory) : Publisher<OrderPurchasedMessage>(factory)
{
    public override string Topic { get; set; } = Constants.PurchaseDoneTopic;
    public override string RoutingKey { get; set; } = "purchase.done";
    
}