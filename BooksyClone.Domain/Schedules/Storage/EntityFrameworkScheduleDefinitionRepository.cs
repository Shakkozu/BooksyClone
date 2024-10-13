using BooksyClone.Domain.Schedules.Shared;
using BooksyClone.Domain.Storage;
using Microsoft.EntityFrameworkCore;

namespace BooksyClone.Domain.Schedules.Storage;

internal interface IScheduleDefinitionRepository
{
    Task SaveAsync(MonthlyScheduleDefinition scheduleDefinition, CancellationToken ct);
    Task<MonthlyScheduleDefinition?> FindAsync(Guid businessUnitId, Guid employeeId, YearMonth yearMonth, CancellationToken ct);
}


internal class EntityFrameworkScheduleDefinitionRepository : IScheduleDefinitionRepository
{
    private readonly SqliteDbContext _sqliteDbContext;

    public EntityFrameworkScheduleDefinitionRepository(SqliteDbContext sqliteDbContext)
    {
        _sqliteDbContext = sqliteDbContext;
    }



    public async Task SaveAsync(MonthlyScheduleDefinition scheduleDefinition, CancellationToken ct)
    {
        _sqliteDbContext.MonthlySchedules.Update(scheduleDefinition);
        await _sqliteDbContext.SaveChangesAsync(ct);
    }

    public async Task<MonthlyScheduleDefinition?> FindAsync(Guid businessUnitId, Guid employeeId, YearMonth yearMonth, CancellationToken ct)
    {
        return await _sqliteDbContext.MonthlySchedules
            .FirstOrDefaultAsync(x =>
                x.BusinessUnitId == businessUnitId &&
                x.EmployeeId == employeeId &&
                x.Year == yearMonth.Year && x.Month == yearMonth.Month, ct);
    }
}
