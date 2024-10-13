using BooksyClone.Domain.Schedules.DefiningSchedules;

namespace BooksyClone.Domain.Schedules.FetchingEmployeeSchedules;

public class EmployeScheduleDto
{
    public Guid EmployeeId { get; set; }
    public string YearMonth { get; set; } //format yyyy-MM
    public IEnumerable<MonthlyScheduleDefinitionDto> Schedule { get; set; }

}