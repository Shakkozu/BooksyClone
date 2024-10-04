using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksyClone.Contract.BusinessUnits;
public class BusinessUnitRegisteredEvent
{
    public Guid BusinessUnitId { get; set; }
    public required IEnumerable<Guid> ManagersId { get; set; }
    public required IEnumerable<Guid> EmployeesId { get; set; }
    public DateTime Timestamp { get; set; }
}
