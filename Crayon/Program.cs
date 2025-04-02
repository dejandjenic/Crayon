using System.Data;
using Crayon.Configuration;
using Crayon.Repositories;
using Crayon.Services;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserAccessorService, UserAccessorService>();
builder.Services.AddScoped<IDBConnectionFactory, DBConnectionFactory>();

var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
builder.Services.AddSingleton<AppSettings>(_ => appSettings);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/accounts", async (IAccountService service,IUserAccessorService userAccessorService) => await service.GetAccounts(await userAccessorService.CurrentCustomer())).WithName("Accounts");
app.MapGet("/accounts/{id}/subscriptions", async (Guid id,IAccountService service,IUserAccessorService userAccessorService) => await service.GetSubscriptions(id)).WithName("AccountSubscriptions");
app.MapGet("/accounts/{id}/subscriptions/{subscriptionId}/licences", async (Guid id, Guid subscriptionId,IAccountService service,IUserAccessorService userAccessorService) => await service.GetLicences(subscriptionId)).WithName("SubscriptionLicences");
app.MapGet("/inventory", (HttpContext ctx) => null).WithName("AvailableInventory");
app.MapPost("/accounts/{id}/subscriptions", async (IAccountService service, IUserAccessorService userAccessorService) =>
{
    
}).WithName("OrderSubscription");
app.MapPatch("/accounts/{id}/subscriptions/{subscriptionId}/quantity", (HttpContext ctx) => null).WithName("UpdateSubscriptionQuantity");
app.MapPatch("/accounts/{id}/subscriptions/{subscriptionId}/expiration", (HttpContext ctx) => null).WithName("UpdateSubscriptionExpiration");
app.MapDelete("/accounts/{id}/subscriptions/{subscriptionId}", (HttpContext ctx) => null).WithName("CancelSubscription");

app.Run();


public partial class Program{}