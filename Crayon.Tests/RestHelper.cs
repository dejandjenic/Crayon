using System.Net;
using System.Net.Http.Json;
using Crayon.ApiClients.CCPClient;
using Crayon.Configuration;
using Crayon.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace TestProject1;

public class RestHelper : IDisposable
{
    private HttpClient client;
    
    public RestHelper(Func<AppSettings,AppSettings> settings)
    {
        var f = new CustomWebApplicationFactory<Program>((services) =>
        {
            var existing = services.BuildServiceProvider().GetRequiredService<AppSettings>();
            services.AddSingleton(sp => settings(existing));
        });
        client = f.CreateClient();
    }

    public void Dispose()
    {
        client.Dispose();
    }

    async Task<(HttpStatusCode, T)> GetResult<T>(string url)
    {
        var res = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
        if (!res.IsSuccessStatusCode)
            return (res.StatusCode, default);
        return (res.StatusCode, await res.Content.ReadFromJsonAsync<T>());
    }

    public async Task<(HttpStatusCode,List<Account>)> Accounts()
    {
        return await GetResult<List<Account>>("/accounts");
    }
    
    public async Task<(HttpStatusCode,List<Subscription>)> Subscriptions(string id)
    {
        return await GetResult<List<Subscription>>($"/accounts/{id}/subscriptions");
    }
    
    public async Task<(HttpStatusCode,List<Licence>)> Licences(string id,string subscriptionid)
    {
        return await GetResult<List<Licence>>($"/accounts/{id}/subscriptions/{subscriptionid}/licences");
    }
    
    public async Task<(HttpStatusCode,List<InventoryItem>)> Inventory()
    {
        return await GetResult<List<InventoryItem>>("/inventory");
    }
}