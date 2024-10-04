using BooksyClone.Domain;
using NUnit.Framework;
using System;
using TechTalk.SpecFlow;

namespace BooksyClone.SpecflowTests.StepDefinitions
{
    [Binding]
    public class LockingScheduleByAnEmployee
    {
        public LockingScheduleByAnEmployee(ScenarioContext context)
        {
            _context = context;
        }

        private Guid _employeeId;
        private Guid _customerId;
        private DailySchedule _dailySchedule;
        private readonly ScenarioContext _context;

        [Given(@"there's day daily schedule with some reservations")]
        public void GivenTheresDayDailyScheduleWithSomeReservations()
        {
            _customerId = Guid.NewGuid();
            var workingHours = TimeSlot.FromDates(
                DateTime.Today.AddHours(8),
                DateTime.Today.AddHours(18));
            _dailySchedule = DailySchedule.From(DateTime.Today,
                _employeeId,
                Guid.NewGuid(),
                new HashSet<Reservation>(),
                workingHours);
            _dailySchedule.Reserve(_customerId, TimeSlot.FromDates(DateTime.Today.AddHours(10), DateTime.Today.AddHours(11)));
            _dailySchedule.Reserve(_customerId, TimeSlot.FromDates(DateTime.Today.AddHours(12), DateTime.Today.AddHours(13)));
            _dailySchedule.Reserve(_customerId, TimeSlot.FromDates(DateTime.Today.AddHours(14), DateTime.Today.AddHours(15)));
        }

        [When(@"employee locks timeslot for which reservations already exists")]
        public void WhenEmployeeLocksTimeslotForWhichReservationsAlreadyExists()
        {
            //var blockadeTimeslot = TimeSlot.FromDates(DateTime.Today.AddHours(12), DateTime.Today.AddHours(24));
            //var result = _dailySchedule.AddBlockade(_employeeId, blockadeTimeslot);
            //_context["addBlockadeResult"] = result;
        }

        [Then(@"lock is created, result contains list of conflicting reservations")]
        public void ThenLockIsCreatedResultContainsListOfConflictingReservations()
        {
            BlockadeCreatedEvent blockadeCreatedEvent = _context.Get< BlockadeCreatedEvent>("addBlockadeResult");
            Assert.IsNotNull(blockadeCreatedEvent);
            blockadeCreatedEvent.ConflictingReservations.Should().BeEquivalentTo(new[]
            {
                new ConflictingReservation(_customerId, TimeSlot.FromDates(DateTime.Today.AddHours(12), DateTime.Today.AddHours(13))),
                new ConflictingReservation(_customerId, TimeSlot.FromDates(DateTime.Today.AddHours(14), DateTime.Today.AddHours(15)))
            });
            
        }

        [When(@"customer tries to book a reservation within loced timeslot")]
        public void WhenCustomerTriesToBookAReservationWithinLocedTimeslot()
        {
            throw new PendingStepException();
        }

        [Then(@"error is throwed due to unavailable timeslot")]
        public void ThenErrorIsThrowedDueToUnavailableTimeslot()
        {
            throw new PendingStepException();
        }

        [When(@"employee releases created lock")]
        public void WhenEmployeeReleasesCreatedLock()
        {
            throw new PendingStepException();
        }

        [When(@"customer tries to create a reservation for timeslot released by a lock")]
        public void WhenCustomerTriesToCreateAReservationForTimeslotReleasedByALock()
        {
            throw new PendingStepException();
        }
    }
}
