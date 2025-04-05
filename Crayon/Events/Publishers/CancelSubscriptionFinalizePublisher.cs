using Crayon.Events.Base;
using Crayon.Events.Handlers;
using RabbitMQ.Client;

namespace Crayon.Events.Publishers;

public class CancelSubscriptionFinalizePublisher(ConnectionFactory factory) : Publisher<CancelSubscriptionFinalizeMessage>(factory)
{
    public override string Topic { get; set; } = Constants.CancelFinalizeTopic;
    public override string RoutingKey { get; set; } = "subscription.cancel.finalize";
    
}