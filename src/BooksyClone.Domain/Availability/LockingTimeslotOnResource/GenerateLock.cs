using BooksyClone.Contract.Availability;
using BooksyClone.Domain.Availability.Storage;
using BooksyClone.Domain.Availability.UpdatingResourceLockingPolicy;
using BooksyClone.Domain.Schedules;
using BooksyClone.Infrastructure.TimeManagement;
using Dapper;

namespace BooksyClone.Domain.Availability.LockingTimeslotOnResource;

internal class GenerateLock
{
	private readonly ResourceLockingLimitationPolicyFactory _factory;
	private readonly ApplyLock _applyLock;

	internal GenerateLock(DbConnectionFactory dbConnectionFactory, ITimeService timeService)
	{
		_factory = new ResourceLockingLimitationPolicyFactory(new GetResourcePolicies(dbConnectionFactory));
		_applyLock = new ApplyLock(dbConnectionFactory, timeService);
	}

	internal async Task<Result> Handle(GenerateNewLockRequest request)
	{
		var policies = await _factory.GetPoliciesForGivenResource(request.CorrelationId, request.Start, request.End);
		var errors = new List<Error>();
		foreach (var policy in policies)
		{
			var result = policy.CanLockResource(request.CorrelationId, request.Start, request.End);
			if (!result.Succeeded)
				errors.AddRange(result.Errors);
		}
		if (errors.Any())
			return Result.ErrorResult(errors);

		return await _applyLock.Handle(request);
	}


	private class ApplyLock(DbConnectionFactory _dbConnectionFactory, ITimeService _timeService)
	{
		internal async Task<Result> Handle(GenerateNewLockRequest request)
		{
			var timerange = TimeSlot.FromDates(request.Start, request.End);
			var sql = @"
	INSERT INTO resource_lock (resource_id, created_by, timestamp, ""from"", ""to"")
	VALUES (@ResourceId, @CreatedBy, @Timestamp, @From, @To)";
			using var connection = _dbConnectionFactory.CreateConnection();
			connection.Open();

			var dao = new
			{
				ResourceId = request.CorrelationId,
				CreatedBy = request.OwnerId,
				Timestamp = _timeService.Now,
				From = timerange.From,
				To = timerange.To
			};

			var result = await connection.ExecuteAsync(sql, dao);
			if (result == 1)
				return Result.Correct();

			throw new InvalidOperationException("something wrong, no data inserted");
		}
	}
}
