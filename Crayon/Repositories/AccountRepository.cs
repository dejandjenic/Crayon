using System.Data;
using Crayon.Services;
using Dapper;

namespace Crayon.Repositories;

public interface IAccountRepository
{
    Task<List<Account>> GetAccounts(Guid idCustomer);
}
public class AccountRepository(IDBConnectionFactory connectionFactory) : IAccountRepository
{
    public async Task<List<Account>> GetAccounts(Guid idCustomer)
    {
        using var connection = connectionFactory.GetConnection();
        
        return (await connection.QueryAsync<Account>("select * from Account where IDCustomer = @idCustomer", new { idCustomer })).ToList();
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
    public string Status { get; set; }
    public DateTime Expires { get; set; }
}

public class Licence
{
    public Guid Id { get; set; }
    public string Value { get; set; }
}