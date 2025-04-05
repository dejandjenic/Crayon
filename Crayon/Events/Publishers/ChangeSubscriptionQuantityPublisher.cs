using Crayon.Events.Base;
using Crayon.Events.Handlers;
using RabbitMQ.Client;

namespace Crayon.Events.Publishers;

public class ChangeSubscriptionQuantityPublisher(ConnectionFactory factory) : Publisher<ChangeSubscriptionQuantityMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeQuantityTopic;
    public override string RoutingKey { get; set; } = "subscription.quantity.start";
    
}