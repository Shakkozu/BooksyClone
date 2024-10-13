using BooksyClone.Domain.BusinessOnboarding.Model;
using BooksyClone.Domain.Schedules.DefiningSchedules;
using BooksyClone.Domain.Schedules.FetchingEmployeeScheduleDetails;
using BooksyClone.Domain.Schedules.FetchingEmployeeSchedules;
using BooksyClone.Domain.Schedules.RegisteringNewBusinessUnit;
using BooksyClone.Domain.Schedules.Shared;
using BooksyClone.Domain.Schedules.Storage;

namespace BooksyClone.Domain.Schedules;

public class SchedulesFacade
{
    internal SchedulesFacade(IScheduleDefinitionRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }
    private static Dictionary<Guid, IEnumerable<Guid>> _businessesEmployeesMap = new(); //todo
    private readonly IScheduleDefinitionRepository _scheduleRepository;

    internal async Task RegisterNewBusinessUnit(RegisterNewBusinesUnitCommand command)
    {
        _businessesEmployeesMap[command.BusinessUnitId] = [command.OwnerId];
        await Task.CompletedTask;
    }

    internal async Task DefineScheduleAsync(Guid businessUnitId, Guid employeeId, MonthlyScheduleDto dto, CancellationToken ct)
    {
        var yearMonth = new YearMonth(dto.ScheduleDate);
        var schedules = await _scheduleRepository.FindAsync(businessUnitId, employeeId, yearMonth, ct);
        if(schedules != null)
        {
            schedules.Update(dto);
        }
        else
        {
            schedules = new MonthlyScheduleDefinition(yearMonth, businessUnitId, employeeId, dto);
        }

        await _scheduleRepository.SaveAsync(schedules, ct);
    }

    internal IEnumerable<EmployeScheduleDto> FetchCompanyEmployeesSchedules(Guid companyIdentifier)
    {
        return _businessesEmployeesMap[companyIdentifier].Select(employeeId =>
            new EmployeScheduleDto {
                EmployeeId = employeeId,
                Schedule = [],
                YearMonth = DateTime.Today.ToString("yyyy-MM")}
        );
    }

    internal async Task<FetchScheduleDefinitionDetailsResponse> FetchEmployeeScheduleDetailsAsync(Guid businessUnitId, Guid employeeId, YearMonth yearMonth, CancellationToken ct)
    {
        var schedule = await _scheduleRepository.FindAsync(businessUnitId, employeeId, yearMonth, ct);
        if (schedule == null)
            return null;

        return new FetchScheduleDefinitionDetailsResponse
        {
            ScheduleData = $"{schedule.Year}-{schedule.Month}",
            Status = schedule.Status.ToString(),
            ScheduleDefinition = schedule.Definition
        };
    }
}
