using BooksyClone.Contract.Schedules;
using BooksyClone.Contract.Shared;
using BooksyClone.Domain.Schedules.FetchingEmployeeScheduleDetails;
using BooksyClone.Domain.Schedules.FetchingEmployeeSchedules;
using BooksyClone.Domain.Schedules.PublishingSchedule;
using BooksyClone.Domain.Schedules.RegisteringNewBusinessUnit;
using BooksyClone.Domain.Schedules.Shared;
using BooksyClone.Domain.Schedules.Storage;
using BooksyClone.Infrastructure.TimeManagement;

namespace BooksyClone.Domain.Schedules;

public class SchedulesFacade
{
    internal SchedulesFacade(IScheduleDefinitionRepository scheduleRepository,
        ISchedulesEventsPublisher schedulesPublisher,
        ISchedulesBusinessEmployesRepository schedulesBusinessEmployesRepository,
        ITimeService timeService)
    {
        _scheduleRepository = scheduleRepository;
        _schedulesPublisher = schedulesPublisher;
        _schedulesBusinessEmployesRepository = schedulesBusinessEmployesRepository;
        _timeService = timeService;
    }
    private readonly IScheduleDefinitionRepository _scheduleRepository;
    private readonly ISchedulesEventsPublisher _schedulesPublisher;
    private readonly ISchedulesBusinessEmployesRepository _schedulesBusinessEmployesRepository;
    private readonly ITimeService _timeService;

    public async Task RegisterNewBusinessUnit(RegisterNewBusinesUnitCommand command, CancellationToken ct)
    {
        await _schedulesBusinessEmployesRepository.RegisterNewBusinessUnit(command.BusinessUnitId, [command.OwnerId], ct);
    }

    public async Task DefineScheduleAsync(Guid businessUnitId, Guid employeeId, MonthlyScheduleDto dto, CancellationToken ct)
    {
        var yearMonth = new YearMonth(dto.ScheduleDate);
        var schedules = await _scheduleRepository.FindAsync(businessUnitId, employeeId, yearMonth, ct);
        if (schedules != null)
        {
            // todo fetch employees assigned to business unit
            schedules.Update(dto);
        }
        else
        {
            schedules = new MonthlyScheduleDefinition(yearMonth, businessUnitId, employeeId, dto);
        }

        await _scheduleRepository.SaveAsync(schedules, ct);
    }

    public async Task<PagedListResponse<EmployeScheduleDto>> FetchCompanyEmployeesSchedules(Guid companyIdentifier, Paging paging, CancellationToken ct)
    {
        var result = await _scheduleRepository.FindByCompanyIdAsync(companyIdentifier, paging, ct);
        var employeesSchedules = result.Items.Select(x => new EmployeScheduleDto
        {
            EmployeeId = x.EmployeeId,
            Schedule = x.Definition,
            Status = x.Status.ToString(),
            YearMonth = $"{x.Year}-{x.Month}"
        }
        );

        return new PagedListResponse<EmployeScheduleDto>(employeesSchedules, paging.Page, paging.PageSize, result.TotalCount);
    }

    public async Task<FetchScheduleDefinitionDetailsResponse> FetchEmployeeScheduleDetailsAsync(Guid businessUnitId, Guid employeeId, YearMonth yearMonth, CancellationToken ct)
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

    internal async Task PublishScheduleAsync(Guid publishedBy, Guid businessUnitId, Guid employeeId, YearMonth yearMonth, CancellationToken ct)
    {
        var schedule = await _scheduleRepository.FindAsync(businessUnitId, employeeId, yearMonth, ct);
        if (schedule == null)
            throw new InvalidOperationException("not found");

        // potential candidate for outbox pattern due to risk of failing after publicating an event
        schedule.Publish(publishedBy);
        await _schedulesPublisher.Send(new EmployeeSchedulePublishedEvent(
            _timeService.Now,
            employeeId,
            businessUnitId,
            yearMonth.ToString(),
            schedule.Definition));
        await _scheduleRepository.SaveAsync(schedule, ct);
    }
}