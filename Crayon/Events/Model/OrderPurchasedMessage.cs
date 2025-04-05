namespace Crayon.Events.Handlers;

public class OrderPurchasedMessage
{
    public DateTime Expires { get; set; }
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public List<Guid> Licences { get; set; }
}