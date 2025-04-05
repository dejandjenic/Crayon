using Crayon.Exceptions;
using Crayon.Repositories;

namespace Crayon.Services;

public interface IAuthorizationService
{
    Task CheckAccountAccess(Guid idAccount, Guid idCustomer);
    Task CheckSubscriptionAccess(Guid idAccount, Guid idCustomer, Guid idSubscription);
}

public class AuthorizationService(IAccountRepository accountRepository,ISubscriptionRepository subscriptionRepository) : IAuthorizationService
{
    public async Task CheckAccountAccess(Guid idAccount, Guid idCustomer)
    {
        var accounts = await accountRepository.GetAccounts(idCustomer);
        if (!accounts.Any(x=>x.Id==idAccount))
        {
            throw new ForbiddenException("You don't have access to this object!");
        }
    }
    
    public async Task CheckSubscriptionAccess(Guid idAccount, Guid idCustomer, Guid idSubscription)
    {
        var accounts = await accountRepository.GetAccounts(idCustomer);
        if (!accounts.Any(x=>x.Id==idAccount))
        {
            throw new ForbiddenException("You don't have access to this object!");
        }

        var subscriptions = await subscriptionRepository.GetSubscriptions(idAccount);

        if (!subscriptions.Any(x => x.Id == idSubscription))
        {
            throw new ForbiddenException("You don't have access to this object!");
        }
    }
}