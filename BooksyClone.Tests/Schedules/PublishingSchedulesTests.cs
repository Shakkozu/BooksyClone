using BooksyClone.Contract.BusinessOnboarding;
using BooksyClone.Contract.Shared;
using BooksyClone.Domain.Schedules.DefiningSchedules;
using BooksyClone.Domain.Schedules.FetchingEmployeeScheduleDetails;
using BooksyClone.Domain.Schedules.FetchingEmployeeSchedules;
using BooksyClone.Infrastructure.RabbitMQStreams.Producing;
using BooksyClone.Infrastructure.TimeManagement;
using BooksyClone.Tests.Infrastructure;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Net.Http.Json;

namespace BooksyClone.Tests.Schedules;
[TestFixture]
internal class PublishingSchedulesTests
{
    private BooksyCloneApp _app;
    private HttpClient _httpClient;
    private Guid _businessUnitId;
    private Guid _businessOwnerId;
    private RabbitMQStreamProducerConfiguration _testProducerConfiguration;
    private TestProducer _testProducer;
    private ITimeService _timeService;
    private PagedListResponse<EmployeScheduleDto> _emptyEmployeesSchedules;

    [OneTimeSetUp]
    public void Setup()
    {
        _businessUnitId = Guid.NewGuid();
        _businessOwnerId = Guid.NewGuid();

        var streamName = "business-units-tests2";
        _testProducerConfiguration = new RabbitMQStreamProducerConfiguration(
            streamName,
            "127.0.0.1",
            5552);
        _testProducer = new TestProducer(_testProducerConfiguration);

        _timeService = A.Fake<ITimeService>();
        A.CallTo(() => _timeService.Now).Returns(DateTime.ParseExact("2024-08-01", "yyyy-MM-dd", CultureInfo.InvariantCulture));


        _app = BooksyCloneApp.CreateInstance(services =>
        {
            services.AddSingleton(_testProducer);
            services.AddSingleton(_timeService);
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
    via POST /companies/{companyIdentifier}/employees/{employeeId}/schedules
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
        WhenManagerDefinesEmployeeMonthlySchedule();
        ThenManagerCanRetrieveCreatedMonthySchedule();
        AndManagerCanViewCreatedMonthlyScheduleOnEmployeeSchedulesList();
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
        _emptyEmployeesSchedules = _httpClient.GetFromJsonAsync<PagedListResponse<EmployeScheduleDto>>($"/api/v1/companies/{_businessUnitId}/employees/schedules").GetAwaiter().GetResult() ?? throw new ArgumentNullException("emptyEmployeeslist");
    }

    private void ThenEmptyListIsReturned()
    {
        Assert.That(_emptyEmployeesSchedules.Items, Is.Empty);
    }

    private void WhenManagerDefinesEmployeeMonthlySchedule()
    {
        var scheduleDefinitionPayload = @"{
	""scheduleDate"": ""2024-10"",
	""scheduleDefinition"": [
		{
			""from"": ""2024-10-01"",
			""to"": ""2024-10-04"",
			""shifts"": [
				{
					""start"": ""09:00"",
					""end"": ""13:00""
				},
				{
					""start"": ""15:00"",
					""end"": ""19:00""
				}
			],
			""description"": """"
		},
		{
			""from"": ""2024-10-05"",
			""to"": ""2024-10-05"",
			""shifts"": [
				{
					""start"": ""10:00"",
					""end"": ""16:00""
				}
			],
			""description"": """"
		},
		{
			""from"": ""2024-10-06"",
			""to"": ""2024-10-06"",
			""shifts"": [],
			""description"": """"
		},
		{
			""from"": ""2024-10-07"",
			""to"": ""2024-10-13"",
			""shifts"": [],
			""description"": ""vacation""
		},
		{
			""from"": ""2024-10-14"",
			""to"": ""2024-10-18"",
			""shifts"": [
				{
					""start"": ""08:00"",
					""end"": ""17:00""
				}
			],
			""description"": """"
		},
		{
			""from"": ""2024-10-21"",
			""to"": ""2024-10-25"",
			""shifts"": [
				{
					""start"": ""08:00"",
					""end"": ""17:00""
				}
			],
			""description"": """"
		},
		{
			""from"": ""2024-10-28"",
			""to"": ""2024-10-31"",
			""shifts"": [
				{
					""start"": ""08:00"",
					""end"": ""17:00""
				}
			],
			""description"": """"
		}
	]
}";
        var content = new StringContent(scheduleDefinitionPayload, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        var response = _httpClient.PostAsync($"/api/v1/companies/{_businessUnitId}/employees/{_businessOwnerId}/schedules", content).GetAwaiter().GetResult();
        var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        Console.WriteLine(responseContent);
        response.EnsureSuccessStatusCode();
    }

    private void ThenManagerCanRetrieveCreatedMonthySchedule()
    {
        var response = _httpClient.GetFromJsonAsync<FetchScheduleDefinitionDetailsResponse>($"/api/v1/companies/{_businessUnitId}/employees/{_businessOwnerId}/schedules/2024-10").GetAwaiter().GetResult();

        Assert.That(response, Is.Not.Null);
        response.Should().BeEquivalentTo(GetFetchScheduleDefinitionDetailsResponse());
    }

    private void AndManagerCanViewCreatedMonthlyScheduleOnEmployeeSchedulesList()
    {
        var response = _httpClient.GetFromJsonAsync<PagedListResponse<EmployeScheduleDto>>($"/api/v1/companies/{_businessUnitId}/employees/schedules").GetAwaiter().GetResult();

        Assert.That(response, Is.Not.Null);
        response.Items.Single().Should().BeEquivalentTo(new EmployeScheduleDto
        {
            EmployeeId = _businessOwnerId,
            Status = "Draft",
            YearMonth = "2024-10",
            Schedule = GetFetchScheduleDefinitionDetailsResponse().ScheduleDefinition
        });
        response.TotalCount.Should().Be(1);
        response.Page.Should().Be(1);
        response.PageSize.Should().Be(100);
    }

    private void WhenManagerPublishesSchedule()
    {

    }

    private void ThenEmployeeSchedulePublishedEventIsPublished()
    {

    }


    private FetchScheduleDefinitionDetailsResponse GetFetchScheduleDefinitionDetailsResponse()
    {
        var definition = new[]
        {
    new MonthlyScheduleDefinitionDto
    {
        From = "2024-10-01",
        To = "2024-10-04",
        Shifts = new[]
        {
            new ShiftDto { Start = "09:00", End = "13:00" },
            new ShiftDto { Start = "15:00", End = "19:00" }
        },
        Description = ""
    },
    new MonthlyScheduleDefinitionDto
    {
        From = "2024-10-05",
        To = "2024-10-05",
        Shifts = new[]
        {
            new ShiftDto { Start = "10:00", End = "16:00" }
        },
        Description = ""
    },
    new MonthlyScheduleDefinitionDto
    {
        From = "2024-10-06",
        To = "2024-10-06",
        Shifts = Array.Empty<ShiftDto>(),
        Description = ""
    },
    new MonthlyScheduleDefinitionDto
    {
        From = "2024-10-07",
        To = "2024-10-13",
        Shifts = Array.Empty<ShiftDto>(),
        Description = "vacation"
    },
    new MonthlyScheduleDefinitionDto
    {
        From = "2024-10-14",
        To = "2024-10-18",
        Shifts = new[]
        {
            new ShiftDto { Start = "08:00", End = "17:00" }
        },
        Description = ""
    },
    new MonthlyScheduleDefinitionDto
    {
        From = "2024-10-21",
        To = "2024-10-25",
        Shifts = new[]
        {
            new ShiftDto { Start = "08:00", End = "17:00" }
        },
        Description = ""
    },
    new MonthlyScheduleDefinitionDto
    {
        From = "2024-10-28",
        To = "2024-10-31",
        Shifts = new[]
        {
            new ShiftDto { Start = "08:00", End = "17:00" }
        },
        Description = ""
    }
};
        var result = new FetchScheduleDefinitionDetailsResponse
        {
            Status = "Draft",
            ScheduleData = "2024-10",
            ScheduleDefinition = definition
        };

        return result;
    }





}
