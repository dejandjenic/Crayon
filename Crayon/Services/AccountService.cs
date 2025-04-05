using Crayon.Entities;
using Crayon.Repositories;

namespace Crayon.Services;

public interface IAccountService
{
    Task<List<Account>> GetAccounts(Guid idCustomer);
}

public class AccountService(IAccountRepository repository) : IAccountService
{
    public async Task<List<Account>> GetAccounts(Guid idCustomer)
    {
        return await repository.GetAccounts(idCustomer);
    }

   
}