namespace Crayon.Events.Handlers;

public class CancelSubscriptionMessage
{
    public Guid SubscriptionId { get; set; }
    public Guid AccountId { get; set; }
}