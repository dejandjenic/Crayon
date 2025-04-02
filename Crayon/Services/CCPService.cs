using Crayon.ApiClients.CCPClient;

namespace Crayon.Services;

public interface ICCPService
{
    Task<List<InventoryItem>> GetInventory();
}

public class CCPService(ICCPClient client) : ICCPService
{
    public async Task<List<InventoryItem>> GetInventory()
    {
        return await client.GetInventory();
    }
}