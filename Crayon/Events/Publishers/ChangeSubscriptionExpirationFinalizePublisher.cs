using Crayon.Events.Base;
using Crayon.Events.Handlers;
using RabbitMQ.Client;

namespace Crayon.Events.Publishers;

public class ChangeSubscriptionExpirationFinalizePublisher(ConnectionFactory factory) : Publisher<ChangeSubscriptionExpirationFinalizeMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeExpirationFinalizeTopic;
    public override string RoutingKey { get; set; } = "subscription.expiration.finalize";
    
}