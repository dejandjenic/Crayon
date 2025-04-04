namespace Crayon.ApiClients.CCPClient.Model;

public class PurchaseResponse
{
    public DateTime Expires { get; set; }
    public List<Guid> Licences { get; set; }
}