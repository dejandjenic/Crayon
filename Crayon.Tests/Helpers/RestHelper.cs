using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Crayon.ApiClients.CCPClient.Model;
using Crayon.Configuration;
using Crayon.Endpoints.Model;
using Crayon.Repositories;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Crayon.Tests.Helpers;

public class SafePortGenerator
{
    public static object lockObject = new();
    public static List<int> ports = new();
    public static int Generate()
    {
        int port = new Random().Next(7001, 7770);
        lock (lockObject)
        {
            while (ports.Contains(port))
            {
                port = new Random().Next(7001, 7770);
            }
            ports.Add(port);
        }

        return port;
    }
}
public class RestHelper : IDisposable
{
    private HttpClient client;
    private string token;
    private CustomWebApplicationFactory<Program> factory;
    private int wiremockport = 0;
    
    public RestHelper(Func<AppSettings,AppSettings> settings,TestJWTLibrary.Generator generator)
    {
        wiremockport = SafePortGenerator.Generate();
        factory = new CustomWebApplicationFactory<Program>((services) =>
        {
            var existing = services.BuildServiceProvider().GetRequiredService<AppSettings>();
            var newSettings = settings(existing);
            newSettings.CCPBaseAddress = $"http://localhost:{wiremockport}";
            newSettings.WiremockPort = wiremockport;
            services.AddSingleton(sp => newSettings);
        },generator);
        
        client = factory.CreateClient();
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }


    public void Authorize(string token)
    {
        this.token = token;
    }
    async Task<(HttpStatusCode, T)> GetResult<T>(string url,HttpMethod method,object body = null)
    {
        var message = new HttpRequestMessage(method, url);
        if (!string.IsNullOrWhiteSpace(token))
        {
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        if (body != null)
        {
            message.Content = JsonContent.Create(body);
        }
        var res = await client.SendAsync(message);
        if (!res.IsSuccessStatusCode)
            return (res.StatusCode, default);

        return (res.StatusCode, await res.Content.ReadFromJsonAsync<T>());
    }
    
    async Task<HttpStatusCode> GetResult(string url,HttpMethod method,object body = null)
    {
        var message = new HttpRequestMessage(method, url);
        if (!string.IsNullOrWhiteSpace(token))
        {
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        if (body != null)
        {
            message.Content = JsonContent.Create(body);
        }
        var res = await client.SendAsync(message);
        
        return res.StatusCode;
    }
    
    async Task<(HttpStatusCode, T)> GetResult<T>(string url,object body = null)
    {
        return await GetResult<T>(url, HttpMethod.Get, body);
    }

    public async Task<(HttpStatusCode,List<Account>)> Accounts()
    {
        return await GetResult<List<Account>>("/accounts");
    }
    
    public async Task<(HttpStatusCode,List<Subscription>)> Subscriptions(Guid id)
    {
        return await GetResult<List<Subscription>>($"/accounts/{id}/subscriptions");
    }
    
    public async Task<(HttpStatusCode,List<Licence>)> Licences(Guid id,Guid subscriptionid)
    {
        return await GetResult<List<Licence>>($"/accounts/{id}/subscriptions/{subscriptionid}/licences");
    }
    
    public async Task<(HttpStatusCode,List<InventoryItem>)> Inventory()
    {
        return await GetResult<List<InventoryItem>>("/inventory");
    }
    
    public async Task<(HttpStatusCode,SubscriptionOrderResponse)> CreateSubscription(Guid id,int quantity, Guid externalId)
    {
        return await GetResult<SubscriptionOrderResponse>($"/accounts/{id}/subscriptions",HttpMethod.Post,new SubscriptionOrderRequest()
        {
            Id = externalId,
            Quantity = quantity
        });
    }
    
    public async Task<HttpStatusCode> ChangeSubscriptionQuantity(Guid accountid,Guid subscriptionId,int quantity)
    {
        return await GetResult($"/accounts/{accountid}/subscriptions/{subscriptionId}/quantity",HttpMethod.Patch,new SubscriptionChangeQuantityRequest()
        {
            Quantity = quantity
        });
    }
    
    public async Task<HttpStatusCode> ChangeSubscriptionExpiration(Guid accountid,Guid subscriptionId,DateTime expires)
    {
        return await GetResult($"/accounts/{accountid}/subscriptions/{subscriptionId}/expiration",HttpMethod.Patch,new SubscriptionChangeExpirationRequest()
        {
            Expires = expires
        });
    }
    
    public async Task<HttpStatusCode> CancelSubscription(Guid accountid,Guid subscriptionId)
    {
        return await GetResult($"/accounts/{accountid}/subscriptions/{subscriptionId}",HttpMethod.Delete);
    }
}