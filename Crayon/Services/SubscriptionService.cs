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
    Task UpdateSubscriptionToPurchased(Guid id, DateTime expires, List<Guid> licences);
    Task ChangeQuantity(Guid id, int quantity);
    Task UpdateSubscriptionChangeQuantity(Guid id, List<Guid> licences);
    Task UpdateSubscriptionError(Guid id, string error);
    Task SetExpiration(Guid id, DateTime expires);
    Task UpdateSubscriptionChangeExpiration(Guid id, DateTime expires);
    Task CancelSubscription(Guid id);
    Task PersistCancellation(Guid id);
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

    public async Task UpdateSubscriptionToPurchased(Guid id,DateTime expires,List<Guid> licences)
    {
        await repository.UpdateSubscriptionToPurchased(id, expires, licences);
    }

    public async Task ChangeQuantity(Guid id, int quantity)
    {
        await repository.UpdateStatusToProcessing(id);
        await changeQuantityPublisher.Publish(new ChangeSubscriptionQuantityMessage()
        {
            SubscriptionId = id,
            Quantity = quantity
        });
    }
    
    public async Task UpdateSubscriptionChangeQuantity(Guid id,List<Guid> licences)
    {
        await repository.UpdateSubscriptionChangeQuantity(id, licences);
    }
    
    public async Task UpdateSubscriptionError(Guid id,string error)
    {
        await repository.UpdateSubscriptionError(id, error);
    }
    
    public async Task SetExpiration(Guid id, DateTime expires)
    {
        await repository.UpdateStatusToProcessing(id);
        await changeExpirationPublisher.Publish(new ChangeSubscriptionExpirationMessage()
        {
            SubscriptionId = id,
            Expires = expires
        });
    }
    
    public async Task UpdateSubscriptionChangeExpiration(Guid id,DateTime expires)
    {
        await repository.UpdateSubscriptionExpiration(id, expires);
    }
    
    public async Task CancelSubscription(Guid id)
    {
        await repository.UpdateStatusToProcessing(id);
        await cancelSubscriptionPublisher.Publish(new CancelSubscriptionMessage()
        {
            SubscriptionId = id
        });
    }
    
    public async Task PersistCancellation(Guid id)
    {
        await repository.UpdateStatusToCancelled(id);
    }
}