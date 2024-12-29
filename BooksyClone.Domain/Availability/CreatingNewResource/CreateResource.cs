using BooksyClone.Contract.Availability;
using BooksyClone.Domain.Availability.Storage;
using Dapper;
using Npgsql;
using System.Data;

namespace BooksyClone.Domain.Availability.CreatingNewResource;

internal class CreateNewResource
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public CreateNewResource(DbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    internal async Task Handle(CreateNewResourceRequest request)
    {
        const string sql = @"
            INSERT INTO resource (guid, correlation_id, owner_id, created_at)
            VALUES (@Guid, @CorrelationId, @OwnerId, @CreatedAt)";
        using var connection = _dbConnectionFactory.CreateConnection();
        connection.Open();

        var resource = new
        {
            CorrelationId = request.CorrelationId,
            Guid = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OwnerId = request.OwnerId,
        };
        await connection.ExecuteAsync(sql, resource);
    }
}
