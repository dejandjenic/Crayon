using System.Data;
using Crayon.Entities;
using Crayon.Services;
using Dapper;

namespace Crayon.Repositories;

public interface ISubscriptionRepository
{
    Task<List<Subscription>> GetSubscriptions(Guid idAccount);
    Task<List<Licence>> GetLicences(Guid idSubscription);
    Task<Guid> CreateSubscription(string name, Guid accountId, Guid externalId);
    Task UpdateSubscriptionToPurchased(Guid id, DateTime expires, List<Guid> licences, Guid accountId);
    Task UpdateStatusToProcessing(Guid id, Guid accountId);
    Task UpdateSubscriptionChangeQuantity(Guid id, List<Guid> licences, Guid accountId);
    Task UpdateSubscriptionError(Guid id, string error, Guid accountId);
    Task UpdateSubscriptionExpiration(Guid id, DateTime expires, Guid accountId);
    Task UpdateStatusToCancelled(Guid id, Guid accountId);
}
public class SubscriptionRepository(IDBConnectionFactory connectionFactory) : ISubscriptionRepository
{

    public async Task<List<Subscription>> GetSubscriptions(Guid idAccount)
    {
        using var connection = connectionFactory.GetConnection();
        
        var data = (await connection.QueryAsync<Subscription>("select * from Subscription where IDAccount = @idAccount", new { idAccount })).ToList();
        return data;
    }

    public async Task<List<Licence>> GetLicences(Guid idSubscription)
    {
        using var connection = connectionFactory.GetConnection();
        
        return (await connection.QueryAsync<Licence>("select * from Licence where IDSubscription = @idSubscription", new { idSubscription })).ToList();
    }

    public async Task<Guid> CreateSubscription(string name,Guid accountId,Guid externalId)
    {
        using var connection = connectionFactory.GetConnection();
        var id = Guid.NewGuid();
        await connection.ExecuteAsync("insert into Subscription (ID,Name,IDAccount,ExternalId,Status) values (@ID,@Name,@IDAccount,@ExternalId,'Created')", new { ID = id, Name = name, IDAccount = accountId,ExternalId = externalId });
        return id;
    }
    
    public async Task UpdateSubscriptionToPurchased(Guid id,DateTime expires,List<Guid> licences, Guid accountId)
    {
        using var connection = connectionFactory.GetConnection();
        using (var tran = connection.BeginTransaction())
        {
            await connection.ExecuteAsync("update Subscription set Status='Active',Expires=@Expires where ID = @ID",
                new { ID = id, Expires = expires }, transaction:tran);

            foreach (var key in licences)
            {
                await connection.ExecuteAsync("insert into Licence (Value,IDSubscription) values (@Value,@IDSubscription)",
                    new { Value = key, IDSubscription = id }, transaction:tran);
            }

            tran.Commit();
        }
    }
    
    
    public async Task UpdateStatusToProcessing(Guid id, Guid accountId)
    {
        using var connection = connectionFactory.GetConnection();
        
        await connection.ExecuteAsync("update Subscription set Status='Processing' where ID = @ID",
            new { ID = id });
    }
    
    public async Task UpdateStatusToCancelled(Guid id, Guid accountId)
    {
        using var connection = connectionFactory.GetConnection();
        
        await connection.ExecuteAsync("update Subscription set Status='Cancelled' where ID = @ID",
            new { ID = id });
    }
    
    public async Task UpdateSubscriptionChangeQuantity(Guid id,List<Guid> licences, Guid accountId)
    {
        using var connection = connectionFactory.GetConnection();
        using (var tran = connection.BeginTransaction())
        {
            await connection.ExecuteAsync("update Subscription set Status='Active' where ID = @ID",
                new { ID = id }, transaction:tran);
            
            await connection.ExecuteAsync("delete from Licence where IDSubscription = @ID",
                new { ID = id }, transaction:tran);

            foreach (var key in licences)
            {
                await connection.ExecuteAsync("insert into Licence (Value,IDSubscription) values (@Value,@IDSubscription)",
                    new { Value = key, IDSubscription = id }, transaction:tran);
            }

            tran.Commit();
        }
    }
    
    public async Task UpdateSubscriptionError(Guid id,string error, Guid accountId)
    {
        using var connection = connectionFactory.GetConnection();
        
        await connection.ExecuteAsync("update Subscription set Status='Error',Error=@Error where ID = @ID",
            new { ID = id, Error = error });
    }
    
    public async Task UpdateSubscriptionExpiration(Guid id,DateTime expires, Guid accountId)
    {
        using var connection = connectionFactory.GetConnection();
        
        await connection.ExecuteAsync("update Subscription set Status='Active',Expires=@Expires where ID = @ID",
            new { ID = id, Expires = expires });
    }
}
