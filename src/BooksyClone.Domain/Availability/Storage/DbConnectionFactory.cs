using Npgsql;
using System.Data;

namespace BooksyClone.Domain.Availability.Storage;

internal class DbConnectionFactory
{
    private readonly string _connectionString;

    internal DbConnectionFactory(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("connection string cannot be empty");
        _connectionString = connectionString;
    }

    internal IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}
