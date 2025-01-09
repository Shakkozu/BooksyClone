using BooksyClone.Domain.Availability.CreatingNewResource;
using BooksyClone.Domain.Availability.LockingTimeslotOnResource;
using BooksyClone.Domain.Availability.Storage;
using BooksyClone.Domain.Availability.UpdatingResourceLockingPolicy;
using BooksyClone.Infrastructure.TimeManagement;
using Microsoft.Extensions.Configuration;

namespace BooksyClone.Domain.Availability;

internal class AvailabilityBuilder
{
    private readonly DbConnectionFactory _dbConnectionFactory;
    private readonly ITimeService _timeService;

    public AvailabilityBuilder(IConfiguration configuration, ITimeService timeService)
    {
        var connectionString = configuration.GetValue<string>(AvailabilityModule.AvailabilityConnectionStringKey);
        if (connectionString is null)
            throw new ArgumentNullException("connectionstring cannot be empty");
        _dbConnectionFactory = new DbConnectionFactory(connectionString);
        _timeService = timeService;
    }

    internal AvailabilityFacade GetFacade()
    {
        return new AvailabilityFacade(
            new CreateNewResource(_dbConnectionFactory),
            new GenerateLock(_dbConnectionFactory, _timeService),
			new UpdateResourcePolicy(_dbConnectionFactory, _timeService),
			new GetResourcePolicies(_dbConnectionFactory)
			);
    }
}
