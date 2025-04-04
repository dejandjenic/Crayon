using Crayon.ApiClients.CCPClient;

namespace Crayon.Services;

public interface ICCPService
{
    Task<List<InventoryItem>> GetInventory();
    Task<PurchaseResponse> Purchase(Guid itemId,Guid accountId,int quantity);
    Task<ChangeQuantityResponse> ChangeQuantity(Guid id, int quantity);
    Task ChangeExpiration(Guid id, DateTime expires);
    Task Cancel(Guid id);
}

public class CCPService(ICCPClient client) : ICCPService
{
    public async Task<List<InventoryItem>> GetInventory()
    {
        return await client.GetInventory();
    }

    public async Task<PurchaseResponse> Purchase(Guid itemId, Guid accountId,int quantity)
    {
        return await client.Purchase(itemId, accountId,quantity);
    }

    public async Task<ChangeQuantityResponse> ChangeQuantity(Guid id, int quantity)
    {
        return await client.ChangeQuantity(id, quantity);
    }
    
    public async Task ChangeExpiration(Guid id, DateTime expires)
    {
        await client.ChangeExpiration(id, expires);
    }
    
    public async Task Cancel(Guid id)
    {
        await client.Cancel(id);
    }
}