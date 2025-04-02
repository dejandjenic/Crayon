using System.Data;
using Crayon.Configuration;
using MySqlConnector;

namespace Crayon.Services;

public interface IDBConnectionFactory
{
    IDbConnection GetConnection();
}
public class DBConnectionFactory(AppSettings appSettings) : IDBConnectionFactory
{
    public IDbConnection GetConnection()
    {
        var connection = new MySqlConnection(appSettings.ConnectionString);
        connection.Open();
        return connection;
    }
}