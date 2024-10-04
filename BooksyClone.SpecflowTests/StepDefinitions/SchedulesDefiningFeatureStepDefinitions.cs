using System;
using TechTalk.SpecFlow;

namespace BooksyClone.SpecflowTests.StepDefinitions
{
    [Binding]
    public class SchedulesDefiningFeatureStepDefinitions
    {
        private Guid _managerId;
        private Guid _businessUnitId;
        private Guid _employeeId;

        [Given(@"a manager assigned to a business unit exists")]
        public async Task GivenAManagerAssignedToABusinessUnitDefinesWorkingHoursForEmployeesForTheUpcomingMonthUsingPOSTCompaniesCompanyIdentifierSchedules()
        {
            var businessUnitsFacade = new BusinessUnitsConfigurator().BusinessUnitsFacade();
            _managerId = Guid.NewGuid();
            _businessUnitId = Guid.NewGuid();
            _employeeId = Guid.NewGuid();
            var registerNewBusinessUnitCommand = new RegisterNewBusinessUnit
            {
                ManagersIds = new[] { _managerId },
                EmployeesIds = new[] { _employeeId },
                BusinessUnitId = _businessUnitId
            };
            await businessUnitsFacade.RegisterNewBusinessUnit(registerNewBusinessUnitCommand);



        }

        [Given(@"when manager defines working hours for employees for the upcoming month using POST /companies/{companyIdentifier}/schedules/")]
        public void WhenManagerDefinesWorkingHoursForEmployeesForTheUpcomingMonth()
        {

        }


        [When(@"the manager publishes the defined schedule using POST /companies/\{companyIdentifier}/schedules/\{createdScheduleIdentifier}/publish")]
        public void WhenTheManagerPublishesTheDefinedScheduleUsingPOSTCompaniesCompanyIdentifierSchedulesCreatedScheduleIdentifierPublish()
        {
            throw new PendingStepException();
        }

        [Then(@"all employees' daily schedules for the defined month are available via GET /companies/\{companyIdentifier}/schedules/\{month-year}")]
        public void ThenAllEmployeesDailySchedulesForTheDefinedMonthAreAvailableViaGETCompaniesCompanyIdentifierSchedulesMonth_Year()
        {
            throw new PendingStepException();
        }

        [Then(@"individual employee daily schedules for the defined month are available via GET /employee/\{employeeId}/schedules/\{dailyScheduleDate}")]
        public void ThenIndividualEmployeeDailySchedulesForTheDefinedMonthAreAvailableViaGETEmployeeEmployeeIdSchedulesDailyScheduleDate()
        {
            throw new PendingStepException();
        }

        [Given(@"a non-manager user tries to publish the schedule using POST /companies/\{companyIdentifier}/schedules/\{createdScheduleIdentifier}/publish")]
        public void GivenANon_ManagerUserTriesToPublishTheScheduleUsingPOSTCompaniesCompanyIdentifierSchedulesCreatedScheduleIdentifierPublish()
        {
            throw new PendingStepException();
        }

        [Then(@"the system returns an error indicating insufficient permissions")]
        public void ThenTheSystemReturnsAnErrorIndicatingInsufficientPermissions()
        {
            throw new PendingStepException();
        }
    }
}
