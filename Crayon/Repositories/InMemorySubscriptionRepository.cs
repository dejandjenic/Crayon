using Crayon.Caching;
using Crayon.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Crayon.Repositories;

public class InMemorySubscriptionRepository(ISubscriptionRepository repository, IMemoryCache cache)
    : InMemoryGenericCache(cache), ISubscriptionRepository
{
    public async Task<List<Subscription>> GetSubscriptions(Guid idAccount)
    {
        return await GetOrSet($"{nameof(SubscriptionRepository)}_{nameof(GetSubscriptions)}_{idAccount}", () => repository.GetSubscriptions(idAccount));
    }

    public async Task<List<Licence>> GetLicences(Guid idSubscription)
    {
        return await GetOrSet($"{nameof(SubscriptionRepository)}_{nameof(GetLicences)}_{idSubscription}", () => repository.GetLicences(idSubscription));
    }

    public async Task<Guid> CreateSubscription(string name, Guid accountId, Guid externalId)
    {
        var result = await repository.CreateSubscription(name, accountId, externalId);
        await RemoveItem($"{nameof(SubscriptionRepository)}_{nameof(GetSubscriptions)}_{accountId}");
        return result;
    }

    public async Task UpdateSubscriptionToPurchased(Guid id, DateTime expires, List<Guid> licences, Guid accountId)
    {
        await repository.UpdateSubscriptionToPurchased(id, expires, licences,accountId);
        await RemoveItem($"{nameof(SubscriptionRepository)}_{nameof(GetSubscriptions)}_{accountId}");
    }

    public async Task UpdateStatusToProcessing(Guid id, Guid accountId)
    {
        await repository.UpdateStatusToProcessing(id,accountId);
        await RemoveItem($"{nameof(SubscriptionRepository)}_{nameof(GetSubscriptions)}_{accountId}");
    }

    public async Task UpdateSubscriptionChangeQuantity(Guid id, List<Guid> licences, Guid accountId)
    {
        await repository.UpdateSubscriptionChangeQuantity(id,licences,accountId);
        await RemoveItem($"{nameof(SubscriptionRepository)}_{nameof(GetSubscriptions)}_{accountId}");
        await RemoveItem($"{nameof(SubscriptionRepository)}_{nameof(GetLicences)}_{id}");
    }

    public async Task UpdateSubscriptionError(Guid id, string error, Guid accountId)
    {
        await repository.UpdateSubscriptionError(id, error,accountId);
        await RemoveItem($"{nameof(SubscriptionRepository)}_{nameof(GetSubscriptions)}_{accountId}");
    }

    public async Task UpdateSubscriptionExpiration(Guid id, DateTime expires, Guid accountId)
    {
        await repository.UpdateSubscriptionExpiration(id,expires,accountId);
        await RemoveItem($"{nameof(SubscriptionRepository)}_{nameof(GetSubscriptions)}_{accountId}");
    }

    public async Task UpdateStatusToCancelled(Guid id, Guid accountId)
    {
        await repository.UpdateStatusToCancelled(id,accountId);
        await RemoveItem($"{nameof(SubscriptionRepository)}_{nameof(GetSubscriptions)}_{accountId}");
    }
}