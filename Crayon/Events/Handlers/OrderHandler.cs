using Crayon.Events.Base;
using Crayon.Events.Publishers;
using Crayon.Services;
using RabbitMQ.Client;

namespace Crayon.Events.Handlers;

public class OrderHandler(ConnectionFactory factory,IServiceScopeFactory serviceScopeFactory) : Subscriber<OrderMessage>(factory)
{
    public override string Topic { get; set; } = Constants.PurchaseTopic;
    public override string BindingKey { get; set; } = "#";

    public async override Task<bool> HandleData(OrderMessage data)
    {
        var scope = serviceScopeFactory.CreateScope();
        var ccpService = scope.ServiceProvider.GetRequiredService<ICCPService>();
        var publisher = scope.ServiceProvider.GetRequiredService<OrderPurchasePublisher>();
        
        var response = await ccpService.Purchase(data.ItemId,data.AccountId,data.Quantity);
        await publisher.Publish(new OrderPurchasedMessage()
        {
            Expires = response.Expires,
            Licences = response.Licences,
            Id = data.SubscriptionId
        });
        
        return true;
    }
}