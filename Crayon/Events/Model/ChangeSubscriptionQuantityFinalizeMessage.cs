namespace Crayon.Events.Handlers;

public class ChangeSubscriptionQuantityFinalizeMessage
{
    public Guid SubscriptionId { get; set; }
    public Guid AccountId { get; set; }
    public List<Guid> Licences { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}