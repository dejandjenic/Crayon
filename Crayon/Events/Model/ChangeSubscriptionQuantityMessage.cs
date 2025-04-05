namespace Crayon.Events.Handlers;

public class ChangeSubscriptionQuantityMessage
{
    public Guid SubscriptionId { get; set; }
    public Guid AccountId { get; set; }
    public int Quantity { get; set; }
}