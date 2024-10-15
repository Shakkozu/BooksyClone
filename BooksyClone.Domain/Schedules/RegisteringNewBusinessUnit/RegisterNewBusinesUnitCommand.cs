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

internal class EntityFrameworkSqliteSchedulesBusinessEmployeesRepository : ISchedulesBusinessEmployesRepository
{
    private readonly SqliteDbContext _sqliteDbContext;

    public EntityFrameworkSqliteSchedulesBusinessEmployeesRepository(SqliteDbContext sqliteDbContext)
    {
        _sqliteDbContext = sqliteDbContext;
    }
    public async Task<IEnumerable<Guid>> GetBusniessEmployees(Guid businessUnitId, CancellationToken ct)
    {
        return (await _sqliteDbContext.ScheduleBusiness
            .SingleAsync(x => x.BusinessUnitId == businessUnitId, ct)).EmployeesIds.ToList();
    }

    public async Task RegisterNewBusinessUnit(Guid businessUnitId, IEnumerable<Guid> employees, CancellationToken ct)
    {
        var business = new ScheduleBusinessUnit {
            BusinessUnitId = businessUnitId,
            EmployeesIds = employees
        };
        _sqliteDbContext.Add(business);
        await _sqliteDbContext.SaveChangesAsync(ct);
    }
}
