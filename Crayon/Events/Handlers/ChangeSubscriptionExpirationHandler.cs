using Crayon.Events.Base;
using Crayon.Events.Publishers;
using Crayon.Services;
using RabbitMQ.Client;

namespace Crayon.Events.Handlers;

public class ChangeSubscriptionExpirationHandler(ConnectionFactory factory,IServiceScopeFactory serviceScopeFactory) : Subscriber<ChangeSubscriptionExpirationMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeExpirationTopic;
    public override string BindingKey { get; set; } = "#";

    public async override Task<bool> HandleData(ChangeSubscriptionExpirationMessage data)
    {
        var scope = serviceScopeFactory.CreateScope();
        
        var ccpService = scope.ServiceProvider.GetRequiredService<ICCPService>();
        var publisher = scope.ServiceProvider.GetRequiredService<ChangeSubscriptionExpirationFinalizePublisher>();

        try
        {
            await ccpService.ChangeExpiration(data.SubscriptionId, data.Expires);

            await publisher.Publish(new ChangeSubscriptionExpirationFinalizeMessage()
            {
                SubscriptionId = data.SubscriptionId,
                Success = true,
                Expires = data.Expires,
                AccountId = data.AccountId
            });
        }
        catch(Exception ex)
        {
            await publisher.Publish(new ChangeSubscriptionExpirationFinalizeMessage()
            {
                SubscriptionId = data.SubscriptionId,
                Success = false,
                Error = ex.Message,
                AccountId = data.AccountId
            });
        }

        return true;
    }
}