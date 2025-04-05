using Crayon.Events.Base;
using Crayon.Events.Handlers;
using RabbitMQ.Client;

namespace Crayon.Events.Publishers;

public class CancelSubscriptionPublisher(ConnectionFactory factory) : Publisher<CancelSubscriptionMessage>(factory)
{
    public override string Topic { get; set; } = Constants.CancelTopic;
    public override string RoutingKey { get; set; } = "subscription.cancel.start";
    
}