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
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
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

builder.Services.AddSingleton<IPublisher,TempPublisher>();
builder.Services.AddSingleton<ISubscriber, TempHandler>();

var app = builder.Build();

var publishers = app.Services.GetRequiredService<IEnumerable<IPublisher>>();
foreach(var publisher in publishers){
    await publisher.Start();
}

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
app.MapGet("/accounts/{id}/subscriptions", async (Guid id,IAccountService service,IUserAccessorService userAccessorService) => await service.GetSubscriptions(id)).WithName("AccountSubscriptions");
app.MapGet("/accounts/{id}/subscriptions/{subscriptionId}/licences", async (Guid id, Guid subscriptionId,IAccountService service,IUserAccessorService userAccessorService) => await service.GetLicences(subscriptionId)).WithName("SubscriptionLicences");
app.MapGet("/inventory", async (ICCPService service) => await service.GetInventory()).WithName("AvailableInventory");
app.MapPost("/accounts/{id}/subscriptions", async (IAccountService service, IUserAccessorService userAccessorService,IPublisher publisher) =>
{
    await publisher.Publish(new TempMessage()
    {
        Id = Guid.NewGuid().ToString()
    });
}).WithName("OrderSubscription");
app.MapPatch("/accounts/{id}/subscriptions/{subscriptionId}/quantity", (HttpContext ctx) => null).WithName("UpdateSubscriptionQuantity");
app.MapPatch("/accounts/{id}/subscriptions/{subscriptionId}/expiration", (HttpContext ctx) => null).WithName("UpdateSubscriptionExpiration");
app.MapDelete("/accounts/{id}/subscriptions/{subscriptionId}", (HttpContext ctx) => null).WithName("CancelSubscription");

MockServer.Start();

app.Run();


public partial class Program{}