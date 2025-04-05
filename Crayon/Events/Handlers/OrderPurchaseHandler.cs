using Crayon.Events.Base;
using Crayon.Services;
using RabbitMQ.Client;

namespace Crayon.Events.Handlers;

public class OrderPurchaseHandler(ConnectionFactory factory,IServiceScopeFactory serviceScopeFactory) : Subscriber<OrderPurchasedMessage>(factory)
{
    public override string Topic { get; set; } = Constants.PurchaseDoneTopic;
    public override string BindingKey { get; set; } = "#";

    public async override Task<bool> HandleData(OrderPurchasedMessage data)
    {
        var scope = serviceScopeFactory.CreateScope();
        var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
        await subscriptionService.UpdateSubscriptionToPurchased(data.Id, data.Expires, data.Licences);
        return true;
    }
}