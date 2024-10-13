using BooksyClone.Domain.Schedules.Storage;

namespace BooksyClone.Domain.Schedules;

internal class SchedulesBuilder
{
    private readonly IScheduleDefinitionRepository _repo;

    public SchedulesBuilder(IScheduleDefinitionRepository repo)
    {
        _repo = repo;
    }
    internal SchedulesFacade GetFacade()
    {
        return new SchedulesFacade(_repo);
    }
}
