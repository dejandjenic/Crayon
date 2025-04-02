using System.Data;
using Crayon.Services;
using Dapper;

namespace Crayon.Repositories;

public interface IAccountRepository
{
    Task<List<Account>> GetAccounts(Guid idCustomer);
    Task<List<Subscription>> GetSubscriptions(Guid idAccount);
    Task<List<Licence>> GetLicences(Guid idSubscription);
}
public class AccountRepository(IDBConnectionFactory connectionFactory) : IAccountRepository
{
    public async Task<List<Account>> GetAccounts(Guid idCustomer)
    {
        using var connection = connectionFactory.GetConnection();
        
        return (await connection.QueryAsync<Account>("select * from Account where IDCustomer = @idCustomer", new { idCustomer })).ToList();
    }

    public async Task<List<Subscription>> GetSubscriptions(Guid idAccount)
    {
        using var connection = connectionFactory.GetConnection();
        
        return (await connection.QueryAsync<Subscription>("select * from Subscription where IDAccount = @idAccount", new { idAccount })).ToList();
    }

    public async Task<List<Licence>> GetLicences(Guid idSubscription)
    {
        using var connection = connectionFactory.GetConnection();
        
        return (await connection.QueryAsync<Licence>("select * from Licence where IDSubscription = @idSubscription", new { idSubscription })).ToList();
    }
}

public class Account
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class Subscription
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class Licence
{
    public Guid Id { get; set; }
    public string Value { get; set; }
}