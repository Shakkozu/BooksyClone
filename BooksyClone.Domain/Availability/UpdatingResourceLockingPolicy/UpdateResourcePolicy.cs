using BooksyClone.Contract.Availability.UpdatingPolicies;
using BooksyClone.Domain.Availability.Storage;
using BooksyClone.Infrastructure.TimeManagement;
using Dapper;
using Newtonsoft.Json;

namespace BooksyClone.Domain.Availability.UpdatingResourceLockingPolicy;

class UpdateResourcePolicy(DbConnectionFactory _dbConnectionFactory,
	ITimeService _timeService)
{
	internal async Task<Result> Handle(UpdateResourceRestrictionsPolicyRequest request)
	{
		var sql = @"
    INSERT INTO resource_locking_restriction_policy (resource_id, created_by, timestamp, ""from"", ""to"", definition)
    VALUES (@ResourceId, @CreatedBy, @Timestamp, @From, @To, @Definition::jsonb)";
		using var connection = _dbConnectionFactory.CreateConnection();
		connection.Open();

		var dao = new
		{
			request.ResourceId,
			request.CreatedBy,
			Timestamp = _timeService.Now,
			From = request.Start.GetStartOfDay(),
			To = request.End.GetEndOfDay(),
			Definition = JsonConvert.SerializeObject(request.Policies)
		};

		var result = await connection.ExecuteAsync(sql, dao);
		if (result == 1)
			return Result.Correct();

		throw new InvalidOperationException("something wrong, no data inserted");
	}
}
