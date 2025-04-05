using Crayon.Events.Base;
using Crayon.Services;
using RabbitMQ.Client;

namespace Crayon.Events.Handlers;

public class ChangeSubscriptionQuantityFinalizeHandler(ConnectionFactory factory,IServiceScopeFactory serviceScopeFactory) : Subscriber<ChangeSubscriptionQuantityFinalizeMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeQuantityFinalizeTopic;
    public override string BindingKey { get; set; } = "#";

    public async override Task<bool> HandleData(ChangeSubscriptionQuantityFinalizeMessage data)
    {
        var scope = serviceScopeFactory.CreateScope();
        
        var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
        
        if(data.Success)
            await subscriptionService.UpdateSubscriptionChangeQuantity(data.SubscriptionId, data.Licences);
        else
            await subscriptionService.UpdateSubscriptionError(data.SubscriptionId, data.Error);

        return true;
    }
}