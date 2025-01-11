using BooksyClone.Contract.Dictionaries;
using BooksyClone.Domain.Availability.Storage;
using Dapper;


namespace BooksyClone.Domain.Dictionaries.FetchingAvailableServicesVariants;
internal class FetchAvailableServiceVariants(DbConnectionFactory _dbConnectionFactory)
{
	public async Task<IEnumerable<ServiceVariantDto>> Handle(CancellationToken ct)
	{
		using var connection = _dbConnectionFactory.CreateConnection();
		var query = @"
            SELECT 
                sv.id AS Id,
                sv.guid AS Guid,
                sv.category_id AS CategoryId,
                sv.name AS Name,
                c.name AS CategoryName,
                sv.created_at AS CreatedAt,
                sv.updated_at AS UpdatedAt
            FROM 
                dictionaries.service_variant sv
            JOIN 
                dictionaries.categories c ON sv.category_id = c.id
            LIMIT 1000";

		var result = await connection.QueryAsync<ServiceVariantDto>(query, ct);

		return await connection.QueryAsync<ServiceVariantDto>(query, ct);
	}
}
