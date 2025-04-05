using Crayon.Events.Base;
using Crayon.Events.Publishers;
using Crayon.Services;
using RabbitMQ.Client;

namespace Crayon.Events.Handlers;

public class CancelSubscriptionHandler(ConnectionFactory factory,IServiceScopeFactory serviceScopeFactory) : Subscriber<CancelSubscriptionMessage>(factory)
{
    public override string Topic { get; set; } = Constants.CancelTopic;
    public override string BindingKey { get; set; } = "#";

    public async override Task<bool> HandleData(CancelSubscriptionMessage data)
    {
        var scope = serviceScopeFactory.CreateScope();
        
        var ccpService = scope.ServiceProvider.GetRequiredService<ICCPService>();
        var publisher = scope.ServiceProvider.GetRequiredService<CancelSubscriptionFinalizePublisher>();

        try
        {
            await ccpService.Cancel(data.SubscriptionId);

            await publisher.Publish(new CancelSubscriptionFinalizeMessage()
            {
                SubscriptionId = data.SubscriptionId,
                Success = true,
                AccountId = data.AccountId
            });
        }
        catch(Exception ex)
        {
            await publisher.Publish(new CancelSubscriptionFinalizeMessage()
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