using BooksyClone.Contract.BusinessUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksyClone.Tests.Schedules;
internal class PublishingSchedulesTests
{
    /*
     * Scenario: Defining and publishing daily working schedules
	    Given a manager assigned to a business unit exists
	    And when manager defines working hours for employees for the upcoming month using POST /companies/{companyIdentifier}/schedules/
	    When the manager publishes the defined schedule using POST /companies/{companyIdentifier}/schedules/{createdScheduleIdentifier}/publish
	    Then all employees' daily schedules for the defined month are available via GET /companies/{companyIdentifier}/schedules/{month-year}
	    And individual employee daily schedules for the defined month are available via GET /employee/{employeeId}/schedules/{dailyScheduleDate}
     */


    [Test]
    public async Task ProcessingBusinessUnitRegisteredEventDefinesBusinessUnitWithEmployeesAndManagers()
    {
        var @event = new BusinessUnitRegisteredEvent
        {
            BusinessUnitId = Guid.NewGuid(),
            ManagersId = new[] { Guid.NewGuid() },
            EmployeesId = new[] { Guid.NewGuid() },
            Timestamp = DateTime.Now,
        };






    }


}
