using Bogus;
using Bogus.Extensions.Poland;
using BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BooksyClone.Tests.BusinessOnboarding;
internal class OnboardingProcessTests
{
    private Faker _generator;
    private RegisterNewBusinessRequest _request;
    private Guid _userId;

    /*
* Scenario: Business onboarding process that creates a business draft
* Given: an application user fills in the registration form to register a new business unit via POST /business-unit
* When: the data provided in the form is correct
* And: the user confirms legal consent that the provided data is accurate
* And: the user uploads an attachment that confirms the existence of the mentioned business
* And: the user uploads an attachment that confirms the user's identity
* Then: a business draft requiring verification is created successfully
* }
* */

    [OneTimeSetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _generator = new Faker("pl");
        _app = 
    }

    [Test]
    public async Task OnboadringProcessScenarioTest()
    {
        GivenApplicationUserFillsRegistrationFormToRegisterANewBusinessWithCorrectData();
        AndUserConfirmedLegalConsentThatProvidedDataIsAccurate();
        AndUserUploadedAnAttachmentThatConfirmsTheExistenceOfMentionedBusiness();
        AndUserUploadedAnAttachmentThatConfirmsTheUsersIdentity();
        ThenBusinessDraftRequiringVerificationIsCreatedSuccessfully();
    }

    private void GivenApplicationUserFillsRegistrationFormToRegisterANewBusinessWithCorrectData()
    {
        _request = new RegisterNewBusinessRequest
        {
            BusinessAddress = _generator.Address.FullAddress(),
            BusinessName = _generator.Company.CompanyName(),
            BusinessType = BusinessType.Barber,
            BusinessEmail = _generator.Internet.Email(),
            BusinessNIP = "7755446779",
            CorrelationId = Guid.NewGuid(),
            Timestamp = DateTime.Today.AddHours(15),
            UserEmail = _generator.Person.Email,
            BusinessPhoneNumber = _generator.Person.Phone,
            UserFullName = _generator.Person.FullName,
            UserIdNumber = _generator.Person.Pesel(),
            UserId = _userId,
            UserPhoneNumber = _generator.Person.Phone,
            LegalConsent = true,
            LegalConsentContent = "Oświadczam że wprowadzone przeze mnie dane są poprawne i zgodne z stanem faktycznym.",
            BusinessProofDocument = CreateFakeFormFile("businessIdentificationDocument.jpg", "test"),
            UserIdentityDocument = CreateFakeFormFile("userIdentity.jpg", "test"),
        };


    }

    private void AndUserConfirmedLegalConsentThatProvidedDataIsAccurate()
    {
        throw new NotImplementedException();
    }
    private void AndUserUploadedAnAttachmentThatConfirmsTheExistenceOfMentionedBusiness()
    {
        throw new NotImplementedException();
    }

    private void AndUserUploadedAnAttachmentThatConfirmsTheUsersIdentity()
    {
        throw new NotImplementedException();
    }



    private void ThenBusinessDraftRequiringVerificationIsCreatedSuccessfully()
    {
        throw new NotImplementedException();
    }


    public IFormFile CreateFakeFormFile(string fileName, string content)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        return new FormFile(stream, 0, stream.Length, "formFile", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };
    }

}
