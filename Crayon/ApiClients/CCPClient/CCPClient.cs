namespace Crayon.ApiClients.CCPClient;

public interface ICCPClient
{
    Task<List<InventoryItem>> GetInventory();
    Task CancelSubscription();
    Task ChangeQuantity();
    Task Purchase();
}

public class CCPClient(HttpClient client) : ICCPClient
{
    public async Task<List<InventoryItem>> GetInventory()
    {
        return await client.GetFromJsonAsync<List<InventoryItem>>("/inventory");
    }

    public Task CancelSubscription()
    {
        throw new NotImplementedException();
    }

    public Task ChangeQuantity()
    {
        throw new NotImplementedException();
    }

    public Task Purchase()
    {
        throw new NotImplementedException();
    }
}

public class InventoryItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}