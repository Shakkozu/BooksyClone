using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Domain.BusinessManagement;
using BooksyClone.Infrastructure.EmailSending;
using FakeItEasy;

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
	// then the invitation code is generated and returned in result to be sent to the employee
	[Test]
	public async Task InvitationLinkShouldBeSentToRegisteredEmployeeEmail()
	{
		var registeredUserId = Guid.NewGuid();
		var businessUnitId = await _app.OnboardingFixture.ABusinessExists(registeredUserId);
		var registerNewEmployeeRequest = new RegisterNewEmployeeRequest
		{
			EmployeeId = Guid.NewGuid(),
			BusinessUnitId = businessUnitId,
			Email = "newEmployee@example.com",
			FirstName = "John",
			LastName = "Doe",
			PhoneNumber = "123456789",
			BirthDate = new DateTime(1990, 1, 1),
			ValidFrom = DateTime.UtcNow,
			ValidTo = DateTime.UtcNow.AddYears(1)
		};

		var result = await facade.RegisterNewEmployeeAsync(registerNewEmployeeRequest, CancellationToken.None);
		
		Assert.IsTrue(result.IsSuccess);
		
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