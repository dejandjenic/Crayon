using Crayon.Entities;
using Crayon.Events.Handlers;
using Crayon.Events.Publishers;
using Crayon.Exceptions;
using Crayon.Repositories;

namespace Crayon.Services;

public interface ISubscriptionService
{
    Task<List<Subscription>> GetSubscriptions(Guid idAccount);
    Task<List<Licence>> GetLicences(Guid idSubscription);
    Task<Guid> Order(Guid customerId, Guid accountId, Guid subscriptionId, int quantity);
    Task UpdateSubscriptionToPurchased(Guid id, DateTime expires, List<Guid> licences, Guid accountId);
    Task ChangeQuantity(Guid id, int quantity, Guid accountId);
    Task UpdateSubscriptionChangeQuantity(Guid id, List<Guid> licences, Guid accountId);
    Task UpdateSubscriptionError(Guid id, string error, Guid accountId);
    Task SetExpiration(Guid id, DateTime expires, Guid accountId);
    Task UpdateSubscriptionChangeExpiration(Guid id, DateTime expires, Guid accountId);
    Task CancelSubscription(Guid id, Guid accountId);
    Task PersistCancellation(Guid id, Guid accountId);
}

public class SubscriptionService(
    ISubscriptionRepository repository,
    OrderPublisher publisher,
    ICCPService ccpService,
    ChangeSubscriptionQuantityPublisher changeQuantityPublisher,
    ChangeSubscriptionExpirationPublisher changeExpirationPublisher,
    CancelSubscriptionPublisher cancelSubscriptionPublisher
    ) : ISubscriptionService
{
    public async Task<List<Subscription>> GetSubscriptions(Guid idAccount)
    {
        return await repository.GetSubscriptions(idAccount);
    }

    public async Task<List<Licence>> GetLicences(Guid idSubscription)
    {
        return await repository.GetLicences(idSubscription);
    }

    public async Task<Guid> Order(Guid customerId,Guid accountId,Guid subscriptionId,int quantity)
    {
        string name = (await ccpService.GetInventory()).FirstOrDefault(x => x.Id == subscriptionId).Name;
        
        var id = await repository.CreateSubscription(name, accountId, subscriptionId);
        await publisher.Publish(new OrderMessage()
        {
            ItemId = subscriptionId,
            AccountId = accountId,
            SubscriptionId = id,
            Quantity = quantity
        });
        return id;
    }

    public async Task UpdateSubscriptionToPurchased(Guid id,DateTime expires,List<Guid> licences, Guid accountId)
    {
        await repository.UpdateSubscriptionToPurchased(id, expires, licences,accountId);
    }

    public async Task ChangeQuantity(Guid id, int quantity, Guid accountId)
    {
        await repository.UpdateStatusToProcessing(id,accountId);
        await changeQuantityPublisher.Publish(new ChangeSubscriptionQuantityMessage()
        {
            SubscriptionId = id,
            Quantity = quantity,
            AccountId = accountId
        });
    }
    
    public async Task UpdateSubscriptionChangeQuantity(Guid id,List<Guid> licences, Guid accountId)
    {
        await repository.UpdateSubscriptionChangeQuantity(id, licences,accountId);
    }
    
    public async Task UpdateSubscriptionError(Guid id,string error, Guid accountId)
    {
        await repository.UpdateSubscriptionError(id, error,accountId);
    }
    
    public async Task SetExpiration(Guid id, DateTime expires, Guid accountId)
    {
        await repository.UpdateStatusToProcessing(id,accountId);
        await changeExpirationPublisher.Publish(new ChangeSubscriptionExpirationMessage()
        {
            SubscriptionId = id,
            Expires = expires,
            AccountId = accountId
        });
    }
    
    public async Task UpdateSubscriptionChangeExpiration(Guid id,DateTime expires, Guid accountId)
    {
        await repository.UpdateSubscriptionExpiration(id, expires,accountId);
    }
    
    public async Task CancelSubscription(Guid id, Guid accountId)
    {
        await repository.UpdateStatusToProcessing(id,accountId);
        await cancelSubscriptionPublisher.Publish(new CancelSubscriptionMessage()
        {
            SubscriptionId = id,
            AccountId = accountId
        });
    }
    
    public async Task PersistCancellation(Guid id, Guid accountId)
    {
        await repository.UpdateStatusToCancelled(id,accountId);
    }
}