﻿using BooksyClone.Domain.Schedules.DefiningSchedules;
using BooksyClone.Domain.Schedules.Shared;
using BooksyClone.Domain.Storage;

namespace BooksyClone.Domain.Schedules.Storage;

internal class MonthlyScheduleDefinition : BaseEntity
{
    internal MonthlyScheduleDefinition() { } // requred for ef
    internal MonthlyScheduleDefinition(YearMonth yearMonth, Guid businessUnitId, Guid employeeId, MonthlyScheduleDto schedule)
    {
        // todo validaiton
        Guid = Guid.NewGuid();
        Definition = schedule.scheduleDefinition;
        Status = ScheduleDefinitionStatus.Draft;
        BusinessUnitId = businessUnitId;
        EmployeeId = employeeId;
        ModifiedAt = DateTime.Now;
        Month = yearMonth.Month;
        Year = yearMonth.Year;
    }

    internal void Update(MonthlyScheduleDto schedule)
    {
        if (Status != ScheduleDefinitionStatus.Draft)
            throw new InvalidOperationException();

        Definition = schedule.scheduleDefinition;
        ModifiedAt = DateTime.Now;
    }

    public int Year { get; set; }
    public int Month { get; set; }
    public ScheduleDefinitionStatus Status { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid BusinessUnitId { get; set; }
    public IEnumerable<MonthlyScheduleDefinitionDto> Definition { get; set; }
    public DateTime ModifiedAt { get; set; }
}