using Crayon.ApiClients.CCPClient.Model;

namespace Crayon.ApiClients.CCPClient;

public interface ICCPClient
{
    Task<List<InventoryItem>> GetInventory();
    Task<PurchaseResponse> Purchase(Guid itemId,Guid accountId,int quantity);
    Task<ChangeQuantityResponse> ChangeQuantity(Guid id, int quantity);
    Task ChangeExpiration(Guid id, DateTime expires);
    Task Cancel(Guid id);
}

public class CCPClient(HttpClient client) : ICCPClient
{
    public async Task<List<InventoryItem>> GetInventory()
    {
        return await client.GetFromJsonAsync<List<InventoryItem>>("/inventory");
    }

    public async Task<PurchaseResponse> Purchase(Guid itemId,Guid accountId,int quantity)
    {
        var res = await client.PostAsJsonAsync("/subscriptions", new PurchaseRequest()
        {
            ItemId = itemId,
            ExternalAccountId = accountId,
            Quantity = quantity
        });
        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<PurchaseResponse>();
    }
    
    public async Task<ChangeQuantityResponse> ChangeQuantity(Guid id, int quantity)
    {
        var res = await client.PatchAsJsonAsync($"/subscriptions/{id}/quantity", new ChangeQuantityRequest()
        {
            Quantity = quantity
        });
        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<ChangeQuantityResponse>();
    }
    
    public async Task ChangeExpiration(Guid id, DateTime expires)
    {
        var res = await client.PatchAsJsonAsync($"/subscriptions/{id}/expiration", new ChangeExpirationRequest()
        {
            Expires = expires
        });
        res.EnsureSuccessStatusCode();
    }
    
    public async Task Cancel(Guid id)
    {
        var res = await client.DeleteAsync($"/subscriptions/{id}");
        res.EnsureSuccessStatusCode();
    }
}