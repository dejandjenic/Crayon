using System.Data;
using Crayon.Entities;
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