namespace BooksyClone.Contract.Schedules;
public record EmployeeSchedulePublishedEvent(
    DateTime Timestamp,
    Guid EmployeeId,
    Guid BusinessUnitId,
    string ScheduleDate,
    IEnumerable<MonthlyScheduleDefinitionDto> ScheduleDefinition
    );


public record MonthlyScheduleDto
{
    public string ScheduleDate { get; set; }
    public MonthlyScheduleDefinitionDto[] ScheduleDefinition { get; set; } = [];
}

public record MonthlyScheduleDefinitionDto
{
    public string From { get; set; }
    public string To { get; set; }
    public ShiftDto[] Shifts { get; set; } = [];
    public string? Description { get; set; }
}

public record ShiftDto
{
    public string Start { get; set; }
    public string End { get; set; }
}
