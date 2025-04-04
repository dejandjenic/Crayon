namespace Crayon.ApiClients.CCPClient.Model;

public class PurchaseRequest
{
    public Guid ItemId { get; set; }
    public Guid ExternalAccountId { get; set; }
    public int Quantity { get; set; }
}