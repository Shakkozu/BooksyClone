using Bogus;
using BooksyClone.Contract.BusinessOnboarding;
using BooksyClone.Contract.BusinessUnits;
using BooksyClone.Domain.Schedules;
using BooksyClone.Infrastructure.RabbitMQStreams;
using BooksyClone.Infrastructure.RabbitMQStreams.Consuming;
using BooksyClone.Infrastructure.RabbitMQStreams.Producing;
using BooksyClone.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace BooksyClone.Tests.Schedules;
[TestFixture]
internal class PublishingSchedulesTests
{
    private Guid _userId;
    private Faker _generator;
    private BooksyCloneApp _app;
    private HttpClient _httpClient;
    private Guid _businessUnitId;
    private Guid _businessOwnerId;
    private RabbitMQStreamProducerConfiguration _testProducerConfiguration;
    private TestProducer _testProducer;
    private IEnumerable<EmployeScheduleDto> _emptyEmployeesSchedules;

    [OneTimeSetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _businessUnitId = Guid.NewGuid();
        _businessOwnerId = Guid.NewGuid();
        _generator = new Faker("pl");

        var streamName = "business-units-tests2";
        _testProducerConfiguration = new RabbitMQStreamProducerConfiguration(
            streamName,
            "127.0.0.1",
            5552);
        _testProducer = new TestProducer(_testProducerConfiguration);

        _app = BooksyCloneApp.CreateInstance(services =>
        {
            services.AddSingleton(_testProducer);
        }
        );
        _httpClient = _app.CreateHttpClient();
    }

    [OneTimeTearDown]
    public void OneTimeTeardown()
    {
        _app.Dispose();
        _httpClient.Dispose();
    }

    /*
 * Scenario: Defining schedules within a business unit
 * 
    Given a business unit draft exists
    When the business owner retrieves employee schedules via GET /companies/{companyIdentifier}/employees/schedules
    Then an empty list is returned

  When the business owner defines the employee's monthly schedule for the upcoming month
    via POST /companies/{companyIdentifier}/employees/{employeeId}/schedules/{year-month}
  Then the schedule is available for viewing 
    via GET /companies/{companyIdentifier}/employees/{employeeId}/schedules/{year-month}
  And the schedule appears in the list of schedules 
    via GET /companies/{companyIdentifier}/employees/{employeeId}/schedules

  When the manager publishes the defined schedule
    via POST /companies/{companyIdentifier}/employees/{employeeId}/schedules/{scheduleIdentifier}/publish
  Then an event "EmployeeSchedulePublished" is emitted, containing the schedule for the specified month
*/


    [Test]
    public void PublishingEmployeeSchedulesTests()
    {
        GivenAnBusinessDraftRegistered();
        WhenManagerFetchesEmployeesSchedules();
        ThenEmptyListIsReturned();
        ManagerCanDefineEmployeeMonthlySchedule();
        ManagerCanRetrieveCreatedMonthySchedule();
        ManagerCanViewCreatedMonthlyScheduleOnEmployeeSchedulesList();
        WhenManagerPublishesSchedule();
        ThenEmployeeSchedulePublishedEventIsPublished();
    }

    private void GivenAnBusinessDraftRegistered()
    {
        _testProducer.Send(new BusinessDraftRegisteredEvent(DateTime.Now, _businessUnitId, _businessOwnerId)).GetAwaiter().GetResult();
        Task.Delay(300).GetAwaiter().GetResult();
    }

    private void WhenManagerFetchesEmployeesSchedules()
    {
        _emptyEmployeesSchedules = _httpClient.GetFromJsonAsync<IEnumerable<EmployeScheduleDto>>($"/api/v1/companies/{_businessUnitId}/employees/schedules").GetAwaiter().GetResult() ?? throw new ArgumentNullException("emptyEmployeeslist");
    }

    private void ThenEmptyListIsReturned()
    {
        Assert.That(_emptyEmployeesSchedules.Single().YearMonth, Is.EqualTo(DateTime.Today.ToString("yyyy-MM")));
        Assert.That(_emptyEmployeesSchedules.Single().EmployeeId, Is.EqualTo(_businessOwnerId));
        Assert.That(_emptyEmployeesSchedules.Single().Schedule, Is.Empty);
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
