using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksyClone.Domain.Schedules;
public class SchedulesFacade
{
    private static Dictionary<Guid, IEnumerable<Guid>> _businessesEmployeesMap = new(); //todo
    public async Task RegisterNewBusinessUnit(RegisterNewBusinesUnitCommand command)
    {
        _businessesEmployeesMap[command.BusinessUnitId] = [command.OwnerId];
        await Task.CompletedTask;
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
}

public record RegisterNewBusinesUnitCommand(Guid BusinessUnitId, Guid OwnerId);


