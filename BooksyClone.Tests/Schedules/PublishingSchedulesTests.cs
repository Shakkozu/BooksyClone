using Bogus;
using BooksyClone.Contract.BusinessOnboarding;
using BooksyClone.Contract.BusinessUnits;
using BooksyClone.Infrastructure.RabbitMQStreams;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BooksyClone.Tests.Schedules;
[TestFixture]
internal class PublishingSchedulesTests
{
    private Guid _userId;
    private Faker _generator;
    private IEventPublisher _fakeEventPublisher;
    private BooksyCloneApp _app;
    private Guid _businessUnitId;

    [OneTimeSetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _businessUnitId = Guid.NewGuid();
        _generator = new Faker("pl");
        _app = BooksyCloneApp.CreateInstance(services =>
        {
        });
    }

    [OneTimeTearDown]
    public void OneTimeTeardown()
    {
        _app.Dispose();
    }

    /*
 * Scenario: Defining employee hierarchy and schedules within a business unit
    Given an business unit draft exists
    And an event "NewEmployeeHired" is received from the message bus
    Then the business owner can retrieve employee schedules via GET /companies/{companyIdentifier}/employees/{employeeId}/schedules, which returns empty list
    When the business owner defines the employee's monthly schedule for the upcoming month via POST /companies/{companyIdentifier}/employees/{employeeId}/schedules/{month-year}
    Then the employee's monthly schedule details are available to view via GET /companies/{companyIdentifier}/employees/{employeeId}/schedules/{month-year}
    And the employee's monthly schedule appears in the list of schedules via GET /companies/{companyIdentifier}/employees/{employeeId}/schedules
    When the manager publishes the defined schedule using POST /companies/{companyIdentifier}/employees/{employeeId}/schedules/{scheduleIdentifier}/publish
    Then an event "EmployeeSchedulePublished" containing the defined schedule for the specified month is published
*/


    [Test]
    public void PublishingEmployeeSchedulesTests()
    {
        GivenAnActivatedBusinessUnitExists();
        AndNewEmployeeHiredEventWasReceived();
        ManagerCanRetrieveEmployeeSchedules();
        ManagerCanDefineEmployeeMonthlySchedule();
        ManagerCanRetrieveCreatedMonthySchedule();
        ManagerCanViewCreatedMonthlyScheduleOnEmployeeSchedulesList();
        WhenManagerPublishesSchedule();
        ThenEmployeeSchedulePublishedEventIsPublished();
    }

    private void GivenAnActivatedBusinessUnitExists()
    {
        var businessPublishedEvent = new BusinessDraftRegisteredEvent(DateTime.Now, _businessUnitId, _userId);
        //_app.SchedulesFacade.RegisterNewBusinessUnit()
        
        


    }

    private void AndNewEmployeeHiredEventWasReceived()
    {

    }

    private void ManagerCanRetrieveEmployeeSchedules()
    {

    }

    private void ManagerCanDefineEmployeeMonthlySchedule()
    {

    }

    private void ManagerCanRetrieveCreatedMonthySchedule()
    {

    }

    private void ManagerCanViewCreatedMonthlyScheduleOnEmployeeSchedulesList()
    {

    }

    private void WhenManagerPublishesSchedule()
    {

    }

    private void ThenEmployeeSchedulePublishedEventIsPublished()
    {

    }








}
