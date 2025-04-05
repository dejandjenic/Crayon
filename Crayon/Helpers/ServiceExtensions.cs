using Crayon.ApiClients.CCPClient;
using Crayon.Configuration;
using Crayon.Events.Base;
using Crayon.Events.Handlers;
using Crayon.Events.Publishers;
using Crayon.Repositories;
using Crayon.Services;
using FluentValidation;
using RabbitMQ.Client;

namespace Crayon.Helpers;

public static class ServiceExtensions
{
    public static void RegisterServices(this IServiceCollection services, AppSettings appSettings)
    {
        
        //services.AddAuthentication();
        services.AddAuthorization();

        services.AddAuthentication("Bearer").AddJwtBearer();
        services.AddHttpContextAccessor();
        
        services.AddOpenApi();

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IUserAccessorService, UserAccessorService>();
        services.AddScoped<IDBConnectionFactory, DBConnectionFactory>();
        services.AddScoped<ICCPService, CCPService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        services.AddHttpClient<ICCPClient, CCPClient>((client) =>
        {
            client.BaseAddress = new Uri(appSettings.CCPBaseAddress);
        });


        services.AddSingleton<AppSettings>(_ => appSettings);
        services.AddSingleton<ConnectionFactory>(_ => new ConnectionFactory()
        {
            HostName = appSettings.PublisherConfiguration.HostName
        });

        services.AddSingleton<OrderPublisher>(sp =>
        {
            var svc = new OrderPublisher(sp.GetRequiredService<ConnectionFactory>());
            svc.Start().GetAwaiter().GetResult();
            return svc;
        });
        services.AddSingleton<ISubscriber, OrderHandler>();
        services.AddSingleton<OrderPurchasePublisher>(sp =>
        {
            var svc = new OrderPurchasePublisher(sp.GetRequiredService<ConnectionFactory>());
            svc.Start().GetAwaiter().GetResult();
            return svc;
        });
        services.AddSingleton<ISubscriber, OrderPurchaseHandler>();
        services.AddSingleton<ChangeSubscriptionQuantityPublisher>(sp =>
        {
            var svc = new ChangeSubscriptionQuantityPublisher(sp.GetRequiredService<ConnectionFactory>());
            svc.Start().GetAwaiter().GetResult();
            return svc;
        });
        services.AddSingleton<ISubscriber, ChangeSubscriptionQuantityHandler>();
        services.AddSingleton<ChangeSubscriptionQuantityFinalizePublisher>(sp =>
        {
            var svc = new ChangeSubscriptionQuantityFinalizePublisher(sp.GetRequiredService<ConnectionFactory>());
            svc.Start().GetAwaiter().GetResult();
            return svc;
        });
        services.AddSingleton<ISubscriber, ChangeSubscriptionQuantityFinalizeHandler>();

        services.AddSingleton<ChangeSubscriptionExpirationPublisher>(sp =>
        {
            var svc = new ChangeSubscriptionExpirationPublisher(sp.GetRequiredService<ConnectionFactory>());
            svc.Start().GetAwaiter().GetResult();
            return svc;
        });
        services.AddSingleton<ISubscriber, ChangeSubscriptionExpirationHandler>();
        services.AddSingleton<ChangeSubscriptionExpirationFinalizePublisher>(sp =>
        {
            var svc = new ChangeSubscriptionExpirationFinalizePublisher(sp.GetRequiredService<ConnectionFactory>());
            svc.Start().GetAwaiter().GetResult();
            return svc;
        });
        services.AddSingleton<ISubscriber, ChangeSubscriptionExpirationFinalizeHandler>();


        services.AddSingleton<CancelSubscriptionPublisher>(sp =>
        {
            var svc = new CancelSubscriptionPublisher(sp.GetRequiredService<ConnectionFactory>());
            svc.Start().GetAwaiter().GetResult();
            return svc;
        });
        services.AddSingleton<ISubscriber, CancelSubscriptionHandler>();

        services.AddSingleton<CancelSubscriptionFinalizePublisher>(sp =>
        {
            var svc = new CancelSubscriptionFinalizePublisher(sp.GetRequiredService<ConnectionFactory>());
            svc.Start().GetAwaiter().GetResult();
            return svc;
        });
        services.AddSingleton<ISubscriber, CancelSubscriptionFinalizeHandler>();
        services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
    }
}