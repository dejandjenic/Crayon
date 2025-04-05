using System.Text;
using MySqlConnector;

namespace Crayon.Tests.Helpers;

public class DatabaseInitializer
{
    public static void Initialize(string connectionString, string databaseName)
    {
        var conn = new MySqlConnection(connectionString);

        conn.Open();

        var sb = new StringBuilder();
        sb.AppendLine($"drop database if exists {databaseName};");
        sb.AppendLine($"create database {databaseName};");
        sb.AppendLine($"use {databaseName};");
        sb.AppendLine(File.ReadAllText("../../../../Schema.sql"));
        sb.AppendLine(File.ReadAllText("../../../../Seed.sql"));
        
        var cmd = new MySqlCommand(sb.ToString(), conn);
        cmd.ExecuteNonQuery();
        
        conn.Close();
        conn.Dispose();
    }
}