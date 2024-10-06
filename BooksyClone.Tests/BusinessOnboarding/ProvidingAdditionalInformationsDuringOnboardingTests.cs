using Bogus;
using Bogus.Extensions.Poland;
using BooksyClone.Contract.BusinessOnboarding;
using BooksyClone.Domain.BusinessOnboarding.FetchingBusinessCreationApplication;
using BooksyClone.Domain.BusinessOnboarding.Model;
using BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Net;
using System.Net.Http.Json;

namespace BooksyClone.Tests.BusinessOnboarding;

/*
    * Feature: User registration and email confirmation
    * 
    * Scenario: Confirming email address provided during new business registration
    * 
    * Given: An application user fills in the registration form to register a new business unit via POST /business-unit
    * And: The system sends an email with an activation code to the email address provided by the user
    * And: The email contains both an activation code and a link to confirm the email address
    * 
    * When: The user clicks the link provided in the email, which redirects them to PUT /business-units/{businessDraftId}/confirm-email?activationCode={activationCode}
    * 
    * Then: The user's email address is successfully confirmed
    * And: The system sends a confirmation email to notify the user that their email has been successfully verified
    */


[TestFixture]

public class ProvidingAdditionalInformationsDuringOnboardingTests
{
    private Guid _userId;
    private Faker _generator;
    private BooksyCloneApp _app;

    [OneTimeSetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _generator = new Faker("pl");
        _app = BooksyCloneApp.CreateInstance();
    }

    [TearDown]
    public void Teardown()
    {
        //_response?.Dispose();
        //_formData?.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTeardown()
    {
        _app.Dispose();
    }




}