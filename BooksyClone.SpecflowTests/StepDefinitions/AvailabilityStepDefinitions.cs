using BooksyClone.Domain;
using NUnit.Framework;
using System;
using TechTalk.SpecFlow;

namespace BooksyClone.SpecflowTests.StepDefinitions
{
    [Binding]
    public class AvailabilityStepDefinitions
    {
        public AvailabilityStepDefinitions(ScenarioContext context)
        {
            _context = context;
        }

        private Guid _employeeId;
        private DailySchedule _dailySchedule;
        private readonly ScenarioContext _context;

        [Given(@"there's day daily schedule of the worker without reservations")]
        public void GivenTheresDayDailyScheduleOfTheWorkerWithoutReservations()
        {
            var workingHours = TimeSlot.FromDates(
                DateTime.Today.AddHours(8),
                DateTime.Today.AddHours(18));
            _dailySchedule = DailySchedule.From(DateTime.Today,
                _employeeId,
                _employeeId,
                new HashSet<Reservation>(),
                workingHours);
        }

        [When(@"customer tries to book a reservation outside working hours scope")]
        public void WhenCustomerTriesToBookAReservationOutsideWorkingHoursScope()
        {
            var reservationTime = TimeSlot.FromDates(
                DateTime.Today.AddHours(7),
                DateTime.Today.AddHours(8));

            try
            {
                _dailySchedule.Reserve(Guid.NewGuid(), reservationTime);
            }
            catch (Exception ex)
            {
                _context["ReservationException"] = ex;
            }
        }

        [Then(@"error is throwed which states that reservation can be created only within working hours")]
        public void ThenErrorIsThrowedWhichStatesThatReservationCanBeCreatedOnlyWithinWorkingHours()
        {
            var exception = _context["ReservationException"] as Exception;

            Assert.IsNotNull(exception, "Expected an exception to be thrown when booking outside working hours.");
            Assert.IsInstanceOf<TimeSlotOutsideBoundariesException>(exception, "Expected InvalidReservationTimeException to be thrown.");
            Assert.AreEqual("Reservation can only be made within working hours", exception.Message, "Exception message mismatch.");

            _context.Clear();
        }

        [When(@"customer tries to book a reservation within a working hours")]
        public void WhenCustomerTriesToBookAReservationWithinAWorkingHours()
        {
            var reservationTime = TimeSlot.FromDates(
                DateTime.Today.AddHours(8),
                DateTime.Today.AddHours(8).AddMinutes(30));

            try
            {
                _dailySchedule.Reserve(Guid.NewGuid(), reservationTime);
            }
            catch (Exception ex)
            {
                _context["ReservationException"] = ex;
            }
        }

        [Then(@"reservation is created successfully")]
        public void ThenReservationIsCreatedSuccessfully()
        {
            Assert.True(!_context.ContainsKey("ReservationException"));
        }

        [When(@"another customer tries to book a reservation with different timeslot")]
        public void WhenAnotherCustomerTriesToBookAReservationWithDifferentTimeslot()
        {
            var reservationTime = TimeSlot.FromDates(
                DateTime.Today.AddHours(8).AddMinutes(30),
                DateTime.Today.AddHours(9));

            try
            {
                _dailySchedule.Reserve(Guid.NewGuid(), reservationTime);
            }
            catch (Exception ex)
            {
                _context["ReservationException"] = ex;
            }
        }

        [When(@"another customer tries to book a reservation with conflicting timeslot")]
        public void WhenAnotherCustomerTriesToBookAReservationWithConflictingTimeslot()
        {
            var reservationTime = TimeSlot.FromDates(
                DateTime.Today.AddHours(8).AddMinutes(15),
                DateTime.Today.AddHours(8).AddMinutes(45));

            try
            {
                _dailySchedule.Reserve(Guid.NewGuid(), reservationTime);
            }
            catch (Exception ex)
            {
                _context["ReservationException"] = ex;
            }
        }

        [Then(@"error is throwed due to already reservaed timeslot")]
        public void ThenErrorIsThrowedDueToAlreadyReservaedTimeslot()
        {
            var exception = _context["ReservationException"] as Exception;
            Assert.IsNotNull(exception, "Expected an exception to be thrown when trying to reserve already reserved timeslot.");
            Assert.IsInstanceOf<InvalidOperationException>(exception, "Expected InvalidOperationException to be thrown.");
            Assert.AreEqual("Cannot reserve already occupied term", exception.Message, "Exception message mismatch.");
        }
    }
}
