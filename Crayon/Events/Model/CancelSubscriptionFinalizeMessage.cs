namespace Crayon.Events.Handlers;

public class CancelSubscriptionFinalizeMessage
{
    public Guid SubscriptionId { get; set; }
    public Guid AccountId { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}