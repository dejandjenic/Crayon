namespace Crayon.Events.Handlers;

public class OrderMessage
{
    public Guid SubscriptionId { get; set; }
    public Guid ItemId { get; set; }
    public Guid AccountId { get; set; }
    public int Quantity { get; set; }
}