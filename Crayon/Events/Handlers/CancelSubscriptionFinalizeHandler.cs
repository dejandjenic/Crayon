using Crayon.Events.Base;
using Crayon.Services;
using RabbitMQ.Client;

namespace Crayon.Events.Handlers;

public class CancelSubscriptionFinalizeHandler(ConnectionFactory factory,IServiceScopeFactory serviceScopeFactory) : Subscriber<CancelSubscriptionFinalizeMessage>(factory)
{
    public override string Topic { get; set; } = Constants.CancelFinalizeTopic;
    public override string BindingKey { get; set; } = "#";

    public async override Task<bool> HandleData(CancelSubscriptionFinalizeMessage data)
    {
        var scope = serviceScopeFactory.CreateScope();
        
        var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
        
        if(data.Success)
            await subscriptionService.PersistCancellation(data.SubscriptionId);
        else
            await subscriptionService.UpdateSubscriptionError(data.SubscriptionId, data.Error);

        return true;
    }
}