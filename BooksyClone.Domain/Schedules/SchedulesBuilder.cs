using BooksyClone.Domain.Schedules.PublishingSchedule;
using BooksyClone.Domain.Schedules.Storage;
using BooksyClone.Infrastructure.TimeManagement;

namespace BooksyClone.Domain.Schedules;

internal class SchedulesBuilder
{
    private readonly IScheduleDefinitionRepository _repo;
    private readonly ISchedulesRabbitStreamsPublisher _schedulesPublisher;
    private readonly ITimeService _timeService;

    public SchedulesBuilder(IScheduleDefinitionRepository repo,
        ISchedulesRabbitStreamsPublisher schedulesPublisher,
        ITimeService timeService)
    {
        _repo = repo;
        _schedulesPublisher = schedulesPublisher;
        _timeService = timeService;
    }
    internal SchedulesFacade GetFacade()
    {
        return new SchedulesFacade(_repo, _schedulesPublisher, _timeService);
    }
}
