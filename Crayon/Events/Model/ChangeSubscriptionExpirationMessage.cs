namespace Crayon.Events.Handlers;

public class ChangeSubscriptionExpirationMessage
{
    public Guid SubscriptionId { get; set; }
    public Guid AccountId { get; set; }
    public DateTime Expires { get; set; }
}