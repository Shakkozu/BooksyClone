using BooksyClone.Domain;
using System;
using TechTalk.SpecFlow;

namespace BooksyClone.SpecflowTests.StepDefinitions
{
    [Binding]
    public class LockingAndReleasingASchedule
    {
        public LockingAndReleasingASchedule(ScenarioContext context)
        {
            _context = context;
        }

        private Guid _employeeId;
        private DailySchedule _dailySchedule;
        private Guid _customerId;
        private readonly ScenarioContext _context;

        [Given(@"an employee's daily schedule for the day (.*) exists with some reservations")]
        public void GivenAnEmployeesDailyScheduleForTheDayExistsWithSomeReservations(string day)
        {
            _employeeId = Guid.NewGuid();
            _customerId = Guid.NewGuid();
            var workingHours = TimeSlot.FromDates(
                DateTime.Today.AddHours(8),
                DateTime.Today.AddHours(18));
            _dailySchedule = DailySchedule.From(DateTime.Today,
                _employeeId,
                Guid.NewGuid(),
                new HashSet<Reservation>(),
                workingHours);
        }

        [When(@"I fetch the employee's daily schedule using GET /employee/\{employeeId}/schedules/(.*)(.*)")]
        public void WhenIFetchTheEmployeesDailyScheduleUsingGETEmployeeEmployeeIdSchedules(Decimal p0, int p1)
        {
            throw new PendingStepException();
        }

        [Then(@"the response shows available timeslots and reserved timeslots")]
        public void ThenTheResponseShowsAvailableTimeslotsAndReservedTimeslots()
        {
            throw new PendingStepException();
        }

        [When(@"the employee creates a blockade for a timeslot using POST /employee/\{employeeId}/schedules/blockades")]
        public void WhenTheEmployeeCreatesABlockadeForATimeslotUsingPOSTEmployeeEmployeeIdSchedulesBlockades()
        {
            throw new PendingStepException();
        }

        [Then(@"a blockade is created")]
        public void ThenABlockadeIsCreated()
        {
            throw new PendingStepException();
        }

        [Then(@"fetching the daily schedule returns the existing blockade alongside reservations, showing any conflicts")]
        public void ThenFetchingTheDailyScheduleReturnsTheExistingBlockadeAlongsideReservationsShowingAnyConflicts()
        {
            throw new PendingStepException();
        }

        [When(@"a customer attempts to book a reservation within the blocked timeslot using POST /reservations")]
        public void WhenACustomerAttemptsToBookAReservationWithinTheBlockedTimeslotUsingPOSTReservations()
        {
            throw new PendingStepException();
        }

        [Then(@"the response returns an error with information that the selected timeslot is unavailable")]
        public void ThenTheResponseReturnsAnErrorWithInformationThatTheSelectedTimeslotIsUnavailable()
        {
            throw new PendingStepException();
        }

        [When(@"the employee releases the blockade using DELETE /employee/\{employeeId}/schedules/blockades/\{blockadeIdentifier}")]
        public void WhenTheEmployeeReleasesTheBlockadeUsingDELETEEmployeeEmployeeIdSchedulesBlockadesBlockadeIdentifier()
        {
            throw new PendingStepException();
        }

        [Then(@"the blockade is removed")]
        public void ThenTheBlockadeIsRemoved()
        {
            throw new PendingStepException();
        }

        [Then(@"fetching the daily schedule shows no blockades and no conflicts")]
        public void ThenFetchingTheDailyScheduleShowsNoBlockadesAndNoConflicts()
        {
            throw new PendingStepException();
        }

        [When(@"the customer attempts to book a reservation for the timeslot that was previously blocked using POST /reservations")]
        public void WhenTheCustomerAttemptsToBookAReservationForTheTimeslotThatWasPreviouslyBlockedUsingPOSTReservations()
        {
            throw new PendingStepException();
        }

        [Then(@"the reservation is successfully created")]
        public void ThenTheReservationIsSuccessfullyCreated()
        {
            throw new PendingStepException();
        }

        [Then(@"the customer can view their reservation under GET /reservations/\{customerId}")]
        public void ThenTheCustomerCanViewTheirReservationUnderGETReservationsCustomerId()
        {
            throw new PendingStepException();
        }
    }
}
