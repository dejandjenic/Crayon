using Crayon.Events.Base;
using Crayon.Events.Handlers;
using RabbitMQ.Client;

namespace Crayon.Events.Publishers;

public class OrderPublisher(ConnectionFactory factory) : Publisher<OrderMessage>(factory)
{
    public override string Topic { get; set; } = Constants.PurchaseTopic;
    public override string RoutingKey { get; set; } = "purchase.created";
    
}

public class OrderPurchasePublisher(ConnectionFactory factory) : Publisher<OrderPurchasedMessage>(factory)
{
    public override string Topic { get; set; } = Constants.PurchaseDoneTopic;
    public override string RoutingKey { get; set; } = "purchase.done";
    
}

public class ChangeSubscriptionQuantityPublisher(ConnectionFactory factory) : Publisher<ChangeSubscriptionQuantityMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeQuantityTopic;
    public override string RoutingKey { get; set; } = "subscription.quantity.start";
    
}

public class ChangeSubscriptionQuantityFinalizePublisher(ConnectionFactory factory) : Publisher<ChangeSubscriptionQuantityFinalizeMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeQuantityFinalizeTopic;
    public override string RoutingKey { get; set; } = "subscription.quantity.finalize";
    
}

public class ChangeSubscriptionExpirationPublisher(ConnectionFactory factory) : Publisher<ChangeSubscriptionExpirationMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeExpirationTopic;
    public override string RoutingKey { get; set; } = "subscription.expiration.start";
    
}

public class ChangeSubscriptionExpirationFinalizePublisher(ConnectionFactory factory) : Publisher<ChangeSubscriptionExpirationFinalizeMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeExpirationFinalizeTopic;
    public override string RoutingKey { get; set; } = "subscription.expiration.finalize";
    
}

public class CancelSubscriptionPublisher(ConnectionFactory factory) : Publisher<CancelSubscriptionMessage>(factory)
{
    public override string Topic { get; set; } = Constants.CancelTopic;
    public override string RoutingKey { get; set; } = "subscription.cancel.start";
    
}

public class CancelSubscriptionFinalizePublisher(ConnectionFactory factory) : Publisher<CancelSubscriptionFinalizeMessage>(factory)
{
    public override string Topic { get; set; } = Constants.CancelFinalizeTopic;
    public override string RoutingKey { get; set; } = "subscription.cancel.finalize";
    
}