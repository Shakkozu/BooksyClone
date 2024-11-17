using BooksyClone.Domain.Schedules.Publishing.PublishingSchedule;
using BooksyClone.Domain.Schedules.RegisteringNewBusinessUnit;
using BooksyClone.Domain.Schedules.Storage;
using BooksyClone.Infrastructure.TimeManagement;

namespace BooksyClone.Domain.Schedules;

internal class SchedulesBuilder
{
    private readonly IScheduleDefinitionRepository _repo;
    private readonly ISchedulesEventsPublisher _schedulesPublisher;
    private readonly ISchedulesBusinessEmployesRepository _schedulesBusinessEmployesRepository;
    private readonly ITimeService _timeService;

    public SchedulesBuilder(IScheduleDefinitionRepository repo,
        ISchedulesEventsPublisher schedulesPublisher,
        ISchedulesBusinessEmployesRepository schedulesBusinessEmployesRepository,
        ITimeService timeService)
    {
        _repo = repo;
        _schedulesPublisher = schedulesPublisher;
        _schedulesBusinessEmployesRepository = schedulesBusinessEmployesRepository;
        _timeService = timeService;

    }
    internal SchedulesFacade GetFacade()
    {
        return new SchedulesFacade(_repo, _schedulesPublisher, _schedulesBusinessEmployesRepository, _timeService);
    }
}
