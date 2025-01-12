using BooksyClone.Contract.Schedules;

namespace BooksyClone.Domain.Schedules.FetchingEmployeeSchedules;

public class EmployeScheduleDto
{
    public Guid EmployeeId { get; set; }
    public string Status { get; set; }

    public string YearMonth { get; set; }
    public IEnumerable<MonthlyScheduleDefinitionDto> Schedule { get; set; }

}