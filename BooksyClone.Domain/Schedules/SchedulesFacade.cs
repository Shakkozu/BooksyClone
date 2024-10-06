using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksyClone.Domain.Schedules;
public class SchedulesFacade
{
    public async Task RegisterNewBusinessUnit(RegisterNewBusinesUnitCommand command)
    {

    }
}

public record RegisterNewBusinesUnitCommand(Guid BusinessUnitId, Guid OwnerId);



