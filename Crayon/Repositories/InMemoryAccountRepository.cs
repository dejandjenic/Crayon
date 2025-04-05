using Crayon.Caching;
using Crayon.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Crayon.Repositories;

public class InMemoryAccountRepository(IAccountRepository repository, IMemoryCache cache)
    : InMemoryGenericCache(cache), IAccountRepository
{
    public async Task<List<Account>> GetAccounts(Guid idCustomer)
    {
        return await GetOrSet($"{nameof(AccountRepository)}_{nameof(GetAccounts)}_{idCustomer}", () => repository.GetAccounts(idCustomer));
    }
}