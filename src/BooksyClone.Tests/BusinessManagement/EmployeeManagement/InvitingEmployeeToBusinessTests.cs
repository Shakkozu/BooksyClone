using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Domain.Auth.Login;
using BooksyClone.Domain.Auth.RegisterUser;
using BooksyClone.Domain.BusinessManagement;
using BooksyClone.Infrastructure.EmailSending;
using FakeItEasy;
using Newtonsoft.Json;
using RabbitMQ.Stream.Client;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace BooksyClone.Tests.BusinessManagement.EmployeeManagement;

public class InvitingEmployeeToBusinessTests
{
    private BooksyCloneApp _app;

    [SetUp]
    public void Setup()
    {
        _app = BooksyCloneApp.CreateInstance();
    }

    [TearDown]
    public void TearDown()
    {
        _app.Dispose();
    }

    // Scenario: Inviting employee to join business
    // Given I am a business owner
    // When I invite an employee to join my business
    // Then the invitation code is generated and returned in result to be sent to the employee
    // And employee might register an account using the invitation code
    // When the employee login with created account
    // Then the employee should be associated with the business
    [Test]
    public async Task RegisteringEmployeeAccountUsingInvitationLinkAcceptanceTests()
    {
        var registeredUserId = Guid.NewGuid();
        var businessUnitId = await _app.OnboardingFixture.ABusinessExists(registeredUserId);
		var password = "Password123!";
        var registerNewEmployeeRequest = new RegisterNewEmployeeRequest
        {
            EmployeeId = Guid.NewGuid(),
            BusinessUnitId = businessUnitId,
            Email = $"{Guid.NewGuid()}-employee@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "123456789",
            BirthDate = new DateTime(1990, 1, 1),
            ValidFrom = DateTime.UtcNow,
            ValidTo = DateTime.UtcNow.AddYears(1)
        };
        var facade = _app.BusinessManagementFacade;
        var registeringNewEmployeeResult = await facade.RegisterNewEmployeeToBusinessAsync(registerNewEmployeeRequest, CancellationToken.None);
        var registerEmployeeAccountUsingTokenRequest = new RegisterEmployeeAccountUsingNewEmployeeTokenRequest
        {
            Email = registerNewEmployeeRequest.Email,
            Password = password,
            ConfirmPassowrd = password,
            PhoneNumber = registerNewEmployeeRequest.PhoneNumber,
            Token = registeringNewEmployeeResult.Token
        };

        var registrationResult = await facade.RegisterEmployeeAccountUsingNewEmployeeTokenAsync(registerEmployeeAccountUsingTokenRequest);
        Assert.That(registrationResult.Succeeded, Is.True);

        var loginDto = new LoginUserDto { Email = registerNewEmployeeRequest.Email, Password = password };
        var loginResponse = await _app.CreateHttpClient().PostAsync("/api/accounts/login", new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json"));
        var responseContent = await loginResponse.Content.ReadAsStringAsync();
        var sessionToken = JsonConvert.DeserializeObject<LoginResponseDto>(responseContent)!.Token;

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/employee/companies");
        request.Headers.Add("Authorization", $"Bearer {sessionToken}");
        var getEmployeeCompaniesResponse = await (await _app.CreateHttpClient().SendAsync(request)).Content.ReadFromJsonAsync<IEnumerable<Guid>>();

        Assert.True(getEmployeeCompaniesResponse!.Single() == businessUnitId);
    }

	[Test]
	public async Task EmployeeWithExistingAccountShouldBeAbleToJoinBusiness()
	{
		var registeredUserId = Guid.NewGuid();
		var businessUnitId = await _app.OnboardingFixture.ABusinessExists(registeredUserId);
		var password = "Password123!";
		var registerNewEmployeeRequest = new RegisterNewEmployeeRequest
		{
			EmployeeId = Guid.NewGuid(),
			BusinessUnitId = businessUnitId,
			Email = $"{Guid.NewGuid()}-employee@example.com",
			FirstName = "John",
			LastName = "Doe",
			PhoneNumber = "123456789",
			BirthDate = new DateTime(1990, 1, 1),
			ValidFrom = DateTime.UtcNow,
			ValidTo = DateTime.UtcNow.AddYears(1)
		};
		var facade = _app.BusinessManagementFacade;
		await _app.AuthFacade.RegisterUserAsync(new UserForRegistrationDto
		{
			ConfirmPassword = password,
			Email = registerNewEmployeeRequest.Email,
			Password = password
		}, CancellationToken.None);
		var loginDto = new LoginUserDto { Email = registerNewEmployeeRequest.Email, Password = password };
		var loginResponse = await _app.CreateHttpClient().PostAsync("/api/accounts/login", new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json"));
		var responseContent = await loginResponse.Content.ReadAsStringAsync();
		var sessionToken = JsonConvert.DeserializeObject<LoginResponseDto>(responseContent)!.Token;
		var registeringNewEmployeeResult = await facade.RegisterNewEmployeeToBusinessAsync(registerNewEmployeeRequest, CancellationToken.None);

		var joinDto = new { registeringNewEmployeeResult.Token };
		var joinRequest = new HttpRequestMessage(HttpMethod.Post, "/api/employee/companies/accept-invitation");
		joinRequest.Headers.Add("Authorization", $"Bearer {sessionToken}");
		joinRequest.Content = new StringContent(JsonConvert.SerializeObject(joinDto), Encoding.UTF8, "application/json");
		var joinBusinessResponse = await _app.CreateHttpClient().SendAsync(joinRequest);
		joinBusinessResponse.EnsureSuccessStatusCode();

		var request = new HttpRequestMessage(HttpMethod.Get, "/api/employee/companies");
		request.Headers.Add("Authorization", $"Bearer {sessionToken}");
		var getEmployeeCompaniesResponse = await (await _app.CreateHttpClient().SendAsync(request)).Content.ReadFromJsonAsync<IEnumerable<Guid>>();

		Assert.True(getEmployeeCompaniesResponse!.Single() == businessUnitId);
	}

    [Test]
    public async Task InvitationShouldExpireAfter72Hours()
    {
    }

    // Scenario: Employee which received an invitation to join business does not have an account
    // Given I am an employee
    // When I receive an invitation to join a business
    // And I do not have an account
    // Then I should be able to create an account
    // Which will be associated with the business
    [Test]
    public async Task EmployeeWithoutAccountReceivesInvitation()
    {
    }

    // Scenario: Employee which received an invitation to join business has an account
    // Given I am an employee
    // When I receive an invitation to join a business
    // And I have an account
    // Then I should be able to accept the invitation
    // And I should be associated with the business
    [Test]
    public async Task EmployeeWithAccountReceivesInvitation()
    {
    }

    // Scenario: Firing an employee from business
    // Given I am a business owner
    // When I fire an employee from my business
    // Then the employee should be removed from the business
    // And lose access to the business data
    [Test]
    public async Task EmployeeShouldBeRemovedFromBusiness()
    {
        // TODO!
    }
}