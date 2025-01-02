using BooksyClone.Contract.Availability;
using BooksyClone.Domain.Availability.Storage;
using BooksyClone.Domain.Availability.UpdatingResourceLockingPolicy;
using BooksyClone.Domain.Schedules;
using BooksyClone.Infrastructure.TimeManagement;
using Dapper;

namespace BooksyClone.Domain.Availability.LockingTimeslotOnResource;

internal class GenerateLock
{
	private readonly GetResourcePolicies _getResourcePolicies;
	private readonly DbConnectionFactory _dbConnectionFactory;
    private readonly ITimeService _timeService;

    public GenerateLock(DbConnectionFactory dbConnectionFactory, ITimeService timeService)
    {
		_getResourcePolicies = new GetResourcePolicies(dbConnectionFactory);
		_dbConnectionFactory = dbConnectionFactory;
        _timeService = timeService;
    }

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