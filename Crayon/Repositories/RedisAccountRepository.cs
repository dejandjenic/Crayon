using Crayon.Caching;
using Crayon.Entities;
using StackExchange.Redis;

namespace Crayon.Repositories;

public class RedisAccountRepository(ConnectionMultiplexer redis, IAccountRepository repository) : RedisGenericCache(redis), IAccountRepository
{
    public async Task<List<Account>> GetAccounts(Guid idCustomer)
    {
        return await GetOrSet($"{nameof(AccountRepository)}_{nameof(GetAccounts)}_{idCustomer}", () => repository.GetAccounts(idCustomer));
    }
}