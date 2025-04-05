namespace Crayon.Events.Handlers;

public class ChangeSubscriptionExpirationMessage
{
    public Guid SubscriptionId { get; set; }
    public DateTime Expires { get; set; }
}