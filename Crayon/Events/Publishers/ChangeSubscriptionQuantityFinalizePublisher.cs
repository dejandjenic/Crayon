using Crayon.Events.Base;
using Crayon.Events.Handlers;
using RabbitMQ.Client;

namespace Crayon.Events.Publishers;

public class ChangeSubscriptionQuantityFinalizePublisher(ConnectionFactory factory) : Publisher<ChangeSubscriptionQuantityFinalizeMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeQuantityFinalizeTopic;
    public override string RoutingKey { get; set; } = "subscription.quantity.finalize";
    
}