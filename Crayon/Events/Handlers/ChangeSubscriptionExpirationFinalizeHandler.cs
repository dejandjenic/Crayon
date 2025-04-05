using Crayon.Events.Base;
using Crayon.Services;
using RabbitMQ.Client;

namespace Crayon.Events.Handlers;

public class ChangeSubscriptionExpirationFinalizeHandler(ConnectionFactory factory,IServiceScopeFactory serviceScopeFactory) : Subscriber<ChangeSubscriptionExpirationFinalizeMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeExpirationFinalizeTopic;
    public override string BindingKey { get; set; } = "#";

    public async override Task<bool> HandleData(ChangeSubscriptionExpirationFinalizeMessage data)
    {
        var scope = serviceScopeFactory.CreateScope();
        
        var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
        
        if(data.Success)
            await subscriptionService.UpdateSubscriptionChangeExpiration(data.SubscriptionId, data.Expires, data.AccountId);
        else
            await subscriptionService.UpdateSubscriptionError(data.SubscriptionId, data.Error, data.AccountId);

        return true;
    }
}