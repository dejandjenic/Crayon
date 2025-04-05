namespace Crayon.Events.Handlers;

public class ChangeSubscriptionExpirationFinalizeMessage
{
    public Guid SubscriptionId { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
    public DateTime Expires { get; set; }
}