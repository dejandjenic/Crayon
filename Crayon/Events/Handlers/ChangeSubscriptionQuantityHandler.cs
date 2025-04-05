using Crayon.Events.Base;
using Crayon.Events.Publishers;
using Crayon.Services;
using RabbitMQ.Client;

namespace Crayon.Events.Handlers;

public class ChangeSubscriptionQuantityHandler(ConnectionFactory factory,IServiceScopeFactory serviceScopeFactory) : Subscriber<ChangeSubscriptionQuantityMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeQuantityTopic;
    public override string BindingKey { get; set; } = "#";

    public async override Task<bool> HandleData(ChangeSubscriptionQuantityMessage data)
    {
        var scope = serviceScopeFactory.CreateScope();
        
        var ccpService = scope.ServiceProvider.GetRequiredService<ICCPService>();
        var publisher = scope.ServiceProvider.GetRequiredService<ChangeSubscriptionQuantityFinalizePublisher>();

        try
        {
            var response = await ccpService.ChangeQuantity(data.SubscriptionId, data.Quantity);

            await publisher.Publish(new ChangeSubscriptionQuantityFinalizeMessage()
            {
                SubscriptionId = data.SubscriptionId,
                Success = true,
                Licences = response.Licences,
                AccountId = data.AccountId
            });
        }
        catch(Exception ex)
        {
            await publisher.Publish(new ChangeSubscriptionQuantityFinalizeMessage()
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