using BooksyClone.Domain.Schedules.Storage;
using BooksyClone.Domain.Storage;
using Microsoft.EntityFrameworkCore;

namespace BooksyClone.Domain.Schedules.RegisteringNewBusinessUnit;

public record RegisterNewBusinesUnitCommand(Guid BusinessUnitId, Guid OwnerId);

internal interface ISchedulesBusinessEmployesRepository
{
    Task<IEnumerable<Guid>> GetBusniessEmployees(Guid businessUnitId, CancellationToken ct);

    Task RegisterNewBusinessUnit(Guid businessUnitId, IEnumerable<Guid> employees, CancellationToken ct);
}

internal class EntityFrameworkSchedulesBusinessEmployeesRepository : ISchedulesBusinessEmployesRepository
{
    private readonly PostgresDbContext _PostgresDbContext;

    public EntityFrameworkSchedulesBusinessEmployeesRepository(PostgresDbContext PostgresDbContext)
    {
        _PostgresDbContext = PostgresDbContext;
    }
    public async Task<IEnumerable<Guid>> GetBusniessEmployees(Guid businessUnitId, CancellationToken ct)
    {
        return (await _PostgresDbContext.ScheduleBusiness
            .SingleAsync(x => x.BusinessUnitId == businessUnitId, ct)).EmployeesIds.ToList();
    }

    public async Task RegisterNewBusinessUnit(Guid businessUnitId, IEnumerable<Guid> employees, CancellationToken ct)
    {
        var business = new ScheduleBusinessUnit {
            BusinessUnitId = businessUnitId,
            EmployeesIds = employees
        };
        _PostgresDbContext.Add(business);
        await _PostgresDbContext.SaveChangesAsync(ct);
    }
}
