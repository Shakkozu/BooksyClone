using BooksyClone.Contract.Availability.UpdatingPolicies;
using BooksyClone.Domain.Availability.Storage;
using BooksyClone.Domain.Shared;
using Dapper;
using Newtonsoft.Json;

namespace BooksyClone.Domain.Availability.UpdatingResourceLockingPolicy;

class GetResourcePolicies(DbConnectionFactory _dbConnectionFactory)
{
	internal async Task<List<BaseTimeRestrictionsPolicyDto>> Handle(Guid resourceId, DateTime from, DateTime to)
	{
		const string sql = @"
            SELECT definition
            FROM resource_locking_restriction_policy
            WHERE resource_id = @ResourceId
            AND ""from"" <= @To
            AND ""to"" >= @From
            ORDER BY timestamp DESC
            LIMIT 1";
		using var connection = _dbConnectionFactory.CreateConnection();
		connection.Open();

		var dao = new
		{
			ResourceId = resourceId,
			From = from.GetStartOfDay(),
			To = to.GetEndOfDay()
		};

		var definition = await connection.QuerySingleOrDefaultAsync<string>(sql, dao);
		if (definition is null)
			return [];

		var settings = new JsonSerializerSettings();
		settings.Converters.Add(new PolicyDtoConverter());

		var policies = JsonConvert.DeserializeObject<List<BaseTimeRestrictionsPolicyDto>>(definition, settings);
		if (policies is null)
			return new List<BaseTimeRestrictionsPolicyDto>();

		return policies;
	}
}
