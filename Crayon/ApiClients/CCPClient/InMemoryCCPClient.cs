using Crayon.ApiClients.CCPClient.Model;
using Crayon.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Crayon.ApiClients.CCPClient;

public class InMemoryCCPClient(ICCPClient repository, IMemoryCache cache)
    : InMemoryGenericCache(cache), ICCPClient
{
    public async Task<List<InventoryItem>> GetInventory()
    {
        return await GetOrSet($"{nameof(CCPClient)}_{nameof(GetInventory)}", () => repository.GetInventory());
    }

    public async Task<PurchaseResponse> Purchase(Guid itemId, Guid accountId, int quantity)
    {
        return await repository.Purchase(itemId, accountId, quantity);
    }

    public async Task<ChangeQuantityResponse> ChangeQuantity(Guid id, int quantity)
    {
        return await repository.ChangeQuantity(id, quantity);
    }

    public async Task ChangeExpiration(Guid id, DateTime expires)
    {
        await repository.ChangeExpiration(id, expires);
    }

    public async Task Cancel(Guid id)
    {
        await repository.Cancel(id);
    }
}