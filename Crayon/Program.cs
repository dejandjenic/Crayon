using System.Data;
using Crayon.ApiClients.CCPClient;
using Crayon.Configuration;
using Crayon.Events.Base;
using Crayon.Events.Handlers;
using Crayon.Events.Publishers;
using Crayon.Mock;
using Crayon.Repositories;
using Crayon.Services;
using MySqlConnector;
using RabbitMQ.Client;
using Constants = Crayon.Events.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IUserAccessorService, UserAccessorService>();
builder.Services.AddScoped<IDBConnectionFactory, DBConnectionFactory>();
builder.Services.AddScoped<ICCPService, CCPService>();

builder.Services.AddHttpClient<ICCPClient,CCPClient>((client) =>
{
    client.BaseAddress = new Uri("http://localhost:7777/");
});

var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
builder.Services.AddSingleton<AppSettings>(_ => appSettings);
builder.Services.AddSingleton<ConnectionFactory>(_ => new ConnectionFactory()
{
    HostName = appSettings.PublisherConfiguration.HostName
});

builder.Services.AddSingleton<OrderPublisher>( sp =>
{
    var svc = new OrderPublisher(sp.GetRequiredService<ConnectionFactory>());
    svc.Start().GetAwaiter().GetResult();
    return svc;
});
builder.Services.AddSingleton<ISubscriber, OrderHandler>();
builder.Services.AddSingleton<OrderPurchasePublisher>( sp =>
{
    var svc = new OrderPurchasePublisher(sp.GetRequiredService<ConnectionFactory>());
    svc.Start().GetAwaiter().GetResult();
    return svc;
});
builder.Services.AddSingleton<ISubscriber, OrderPurchaseHandler>();
builder.Services.AddSingleton<ChangeSubscriptionQuantityPublisher>( sp =>
{
    var svc = new ChangeSubscriptionQuantityPublisher(sp.GetRequiredService<ConnectionFactory>());
    svc.Start().GetAwaiter().GetResult();
    return svc;
});
builder.Services.AddSingleton<ISubscriber, ChangeSubscriptionQuantityHandler>();
builder.Services.AddSingleton<ChangeSubscriptionQuantityFinalizePublisher>( sp =>
{
    var svc = new ChangeSubscriptionQuantityFinalizePublisher(sp.GetRequiredService<ConnectionFactory>());
    svc.Start().GetAwaiter().GetResult();
    return svc;
});
builder.Services.AddSingleton<ISubscriber, ChangeSubscriptionQuantityFinalizeHandler>();

builder.Services.AddSingleton<ChangeSubscriptionExpirationPublisher>( sp =>
{
    var svc = new ChangeSubscriptionExpirationPublisher(sp.GetRequiredService<ConnectionFactory>());
    svc.Start().GetAwaiter().GetResult();
    return svc;
});
builder.Services.AddSingleton<ISubscriber, ChangeSubscriptionExpirationHandler>();
builder.Services.AddSingleton<ChangeSubscriptionExpirationFinalizePublisher>( sp =>
{
    var svc = new ChangeSubscriptionExpirationFinalizePublisher(sp.GetRequiredService<ConnectionFactory>());
    svc.Start().GetAwaiter().GetResult();
    return svc;
});
builder.Services.AddSingleton<ISubscriber, ChangeSubscriptionExpirationFinalizeHandler>();


builder.Services.AddSingleton<CancelSubscriptionPublisher>( sp =>
{
    var svc = new CancelSubscriptionPublisher(sp.GetRequiredService<ConnectionFactory>());
    svc.Start().GetAwaiter().GetResult();
    return svc;
});
builder.Services.AddSingleton<ISubscriber, CancelSubscriptionHandler>();

builder.Services.AddSingleton<CancelSubscriptionFinalizePublisher>( sp =>
{
    var svc = new CancelSubscriptionFinalizePublisher(sp.GetRequiredService<ConnectionFactory>());
    svc.Start().GetAwaiter().GetResult();
    return svc;
});
builder.Services.AddSingleton<ISubscriber, CancelSubscriptionFinalizeHandler>();

var app = builder.Build();

var subscribers = app.Services.GetRequiredService<IEnumerable<ISubscriber>>();
foreach (var subscriber in subscribers)
{
    await subscriber.Start();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/accounts", async (IAccountService service,IUserAccessorService userAccessorService) => await service.GetAccounts(await userAccessorService.CurrentCustomer())).WithName("Accounts");
app.MapGet("/accounts/{id}/subscriptions", async (Guid id,ISubscriptionService service,IUserAccessorService userAccessorService) => await service.GetSubscriptions(id)).WithName("AccountSubscriptions");
app.MapGet("/accounts/{id}/subscriptions/{subscriptionId}/licences", async (Guid id, Guid subscriptionId,ISubscriptionService service,IUserAccessorService userAccessorService) => await service.GetLicences(subscriptionId)).WithName("SubscriptionLicences");
app.MapGet("/inventory", async (ICCPService service) => await service.GetInventory()).WithName("AvailableInventory");
app.MapPost("/accounts/{id}/subscriptions", async (ISubscriptionService service, IUserAccessorService userAccessorService,Guid id,SubscriptionOrderRequest request) =>
{
    var subscriptionId = await service.Order(await userAccessorService.CurrentCustomer(),id,request.Id,request.Quantity);
    return Results.Created($"/accounts/{id}/subscriptions/{subscriptionId}", new SubscriptionOrderResponse
    {
        Id = subscriptionId
    });
}).WithName("OrderSubscription");
app.MapPatch("/accounts/{id}/subscriptions/{subscriptionId}/quantity", async (ISubscriptionService service,SubscriptionChangeQuantityRequest request,Guid subscriptionId) =>
{
    await service.ChangeQuantity(subscriptionId, request.Quantity);
    return Results.Accepted();
}).WithName("UpdateSubscriptionQuantity");
app.MapPatch("/accounts/{id}/subscriptions/{subscriptionId}/expiration", 
    async (ISubscriptionService service,Guid subscriptionId,SubscriptionChangeExpirationRequest request) =>
{
    await service.SetExpiration(subscriptionId,request.Expires);
    return Results.Accepted();
}).WithName("UpdateSubscriptionExpiration");
app.MapDelete("/accounts/{id}/subscriptions/{subscriptionId}",
    async (ISubscriptionService service, Guid subscriptionId) =>
    {
        await service.CancelSubscription(subscriptionId);
        return Results.Accepted();
    }).WithName("CancelSubscription");

MockServer.Start();

app.Run();

public partial class Program{}

public class SubscriptionOrderRequest
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
}

public class SubscriptionChangeQuantityRequest
{
    public int Quantity { get; set; }
}

public class SubscriptionChangeExpirationRequest
{
    public DateTime Expires { get; set; }
}

public class SubscriptionOrderResponse
{
    public Guid Id { get; set; }
}
