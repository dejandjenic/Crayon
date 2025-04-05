using Crayon.Events.Base;
using Crayon.Events.Handlers;
using RabbitMQ.Client;

namespace Crayon.Events.Publishers;

public class ChangeSubscriptionExpirationPublisher(ConnectionFactory factory) : Publisher<ChangeSubscriptionExpirationMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeExpirationTopic;
    public override string RoutingKey { get; set; } = "subscription.expiration.start";
    
}