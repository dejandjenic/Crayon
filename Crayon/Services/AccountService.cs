using Crayon.Repositories;

namespace Crayon.Services;

public interface IAccountService
{
    Task<List<Account>> GetAccounts(Guid idCustomer);
    Task<List<Subscription>> GetSubscriptions(Guid idAccount);
    Task<List<Licence>> GetLicences(Guid idSubscription);
}

public class AccountService(IAccountRepository repository) : IAccountService
{
    public async Task<List<Account>> GetAccounts(Guid idCustomer)
    {
        return await repository.GetAccounts(idCustomer);
    }

    public async Task<List<Subscription>> GetSubscriptions(Guid idAccount)
    {
        return await repository.GetSubscriptions(idAccount);
    }

    public async Task<List<Licence>> GetLicences(Guid idSubscription)
    {
        return await repository.GetLicences(idSubscription);
    }
}