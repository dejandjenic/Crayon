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
                Status = "Active",
                Licences = response.Licences
            });
        }
        catch(Exception ex)
        {
            await publisher.Publish(new ChangeSubscriptionQuantityFinalizeMessage()
            {
                SubscriptionId = data.SubscriptionId,
                Status = "Error",
                Error = ex.Message
            });
        }

        return true;
    }
}

public class ChangeSubscriptionQuantityFinalizeHandler(ConnectionFactory factory,IServiceScopeFactory serviceScopeFactory) : Subscriber<ChangeSubscriptionQuantityFinalizeMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeQuantityFinalizeTopic;
    public override string BindingKey { get; set; } = "#";

    public async override Task<bool> HandleData(ChangeSubscriptionQuantityFinalizeMessage data)
    {
        var scope = serviceScopeFactory.CreateScope();
        
        var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
        
        if(data.Status == "Active")
            await subscriptionService.UpdateSubscriptionChangeQuantity(data.SubscriptionId, data.Licences);
        else
            await subscriptionService.UpdateSubscriptionError(data.SubscriptionId, data.Error);

        return true;
    }
}


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
                Status = "Active",
                Expires = data.Expires
            });
        }
        catch(Exception ex)
        {
            await publisher.Publish(new ChangeSubscriptionExpirationFinalizeMessage()
            {
                SubscriptionId = data.SubscriptionId,
                Status = "Error",
                Error = ex.Message
            });
        }

        return true;
    }
}

public class ChangeSubscriptionExpirationFinalizeHandler(ConnectionFactory factory,IServiceScopeFactory serviceScopeFactory) : Subscriber<ChangeSubscriptionExpirationFinalizeMessage>(factory)
{
    public override string Topic { get; set; } = Constants.ChangeExpirationFinalizeTopic;
    public override string BindingKey { get; set; } = "#";

    public async override Task<bool> HandleData(ChangeSubscriptionExpirationFinalizeMessage data)
    {
        var scope = serviceScopeFactory.CreateScope();
        
        var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
        
        if(data.Status == "Active")
            await subscriptionService.UpdateSubscriptionChangeExpiration(data.SubscriptionId, data.Expires);
        else
            await subscriptionService.UpdateSubscriptionError(data.SubscriptionId, data.Error);

        return true;
    }
}



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
                Status = "Active",
            });
        }
        catch(Exception ex)
        {
            await publisher.Publish(new CancelSubscriptionFinalizeMessage()
            {
                SubscriptionId = data.SubscriptionId,
                Status = "Error",
                Error = ex.Message
            });
        }

        return true;
    }
}

public class CancelSubscriptionFinalizeHandler(ConnectionFactory factory,IServiceScopeFactory serviceScopeFactory) : Subscriber<CancelSubscriptionFinalizeMessage>(factory)
{
    public override string Topic { get; set; } = Constants.CancelFinalizeTopic;
    public override string BindingKey { get; set; } = "#";

    public async override Task<bool> HandleData(CancelSubscriptionFinalizeMessage data)
    {
        var scope = serviceScopeFactory.CreateScope();
        
        var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
        
        if(data.Status == "Active")
            await subscriptionService.PersistCancellation(data.SubscriptionId);
        else
            await subscriptionService.UpdateSubscriptionError(data.SubscriptionId, data.Error);

        return true;
    }
}