using Bogus;
using Bogus.Extensions.Poland;
using BooksyClone.Contract.BusinessOnboarding;
using BooksyClone.Domain.BusinessOnboarding;
using BooksyClone.Domain.BusinessOnboarding.FetchingBusinessCreationApplication;
using BooksyClone.Domain.BusinessOnboarding.Model;
using BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;
using BooksyClone.Infrastructure.RabbitMQStreams;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Net;
using System.Net.Http.Json;

namespace BooksyClone.Tests.BusinessOnboarding;
[TestFixture]
public class OnboardingProcessUserAcceptanceTests
{
    private const string _legalConsent = "Oświadczam że wprowadzone przeze mnie dane są poprawne i zgodne z stanem faktycznym.";
    private Faker _generator;
    private IOnboardingEventsPublisher _fakeEventPublisher;
    private BooksyCloneApp _app;
    private RegisterNewBusinessRequest _request;
    private IFormFile _businessProofDocument;
    private IFormFile _userIdentityDocument;
    private Guid _userId;
    private MultipartFormDataContent _formData;
    private HttpResponseMessage _response;
    private Guid _businessDraftId;

    [OneTimeSetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _generator = new Faker("pl");
        _fakeEventPublisher = A.Fake<IOnboardingEventsPublisher>();
        _app = BooksyCloneApp.CreateInstance(services =>
        {
            services.AddSingleton<IOnboardingEventsPublisher>(_fakeEventPublisher);
        });
    }

    [TearDown]
    public void Teardown()
    {
        _response?.Dispose();
        _formData?.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTeardown()
    {
        _app.Dispose();
    }


    /*
* Scenario: Business onboarding process that creates a business draft
* Given: an application user fills in the registration form to register a new business unit via POST /business-unit
* When: the data provided in the form is correct
* And: the user confirms legal consent that the provided data is accurate
* And: the user uploads an attachment that confirms the existence of the mentioned business
* And: the user uploads an attachment that confirms the user's identity
* Then: a business draft requiring verification is created successfully
* And: Creator of the business draft can fetch it via GET /business-unit/{identifier}
* And: Information that business draft was created is published on message bus
* }
* */

    [Test]
    public void OnboadringProcessScenarioTest()
    {
        GivenApplicationUserFillsRegistrationFormToRegisterANewBusinessWithCorrectData();
        AndUserConfirmedLegalConsentThatProvidedDataIsAccurate();
        AndUserUploadedAnAttachmentThatConfirmsTheExistenceOfMentionedBusiness();
        AndUserUploadedAnAttachmentThatConfirmsTheUsersIdentity();
        ThenBusinessDraftRequiringVerificationIsCreatedSuccessfully();
        AndCreatorOfTheBusinessDraftCanFetchIt();
        AndInformationThatBusinessDraftWasRegisteredIsPublishedOnMessageBus();
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
            Timestamp = DateTime.Today.AddHours(15).ToUniversalTime(),
            UserEmail = _generator.Person.Email,
            BusinessPhoneNumber = _generator.Phone.PhoneNumber("#########"),
            UserFullName = _generator.Person.FullName,
            UserIdNumber = _generator.Person.Pesel(),
            UserId = _userId,
            UserPhoneNumber = _generator.Phone.PhoneNumber("#########"),
            LegalConsent = true,
            LegalConsentContent = _legalConsent,
        };
        _businessProofDocument = CreateFakeFormFile("businessIdentificationDocument.jpg", "test");
        _userIdentityDocument = CreateFakeFormFile("userIdentity.jpg", "test");
        var httpClient = _app.CreateHttpClient();

        _formData = new MultipartFormDataContent
        {
            { new StreamContent(_businessProofDocument.OpenReadStream()), "BusinessProofDocument", _businessProofDocument.FileName },
            { new StreamContent(_userIdentityDocument.OpenReadStream()), "UserIdentityDocument", _userIdentityDocument.FileName },
             { new StringContent(_request.CorrelationId.ToString()), "CorrelationId" },
            { new StringContent(_request.Timestamp.ToString("O")), "Timestamp" }, // ISO 8601 format
            { new StringContent(_request.BusinessName), "BusinessName" },
            { new StringContent(_request.BusinessType.ToString()), "BusinessType" }, // Assuming it's an enum
            { new StringContent(_request.BusinessNIP), "BusinessNIP" },
            { new StringContent(_request.BusinessAddress), "BusinessAddress" },
            { new StringContent(_request.BusinessPhoneNumber), "BusinessPhoneNumber" },
            { new StringContent(_request.BusinessEmail), "BusinessEmail" },
            { new StringContent(_request.UserId.ToString()), "UserId" },
            { new StringContent(_request.UserFullName), "UserFullName" },
            { new StringContent(_request.UserIdNumber), "UserIdNumber" },
            { new StringContent(_request.UserEmail), "UserEmail" },
            { new StringContent(_request.UserPhoneNumber), "UserPhoneNumber" },
            { new StringContent(_request.LegalConsent.ToString()), "LegalConsent" },
            { new StringContent(_request.LegalConsentContent), "LegalConsentContent" },

        };


        _response = httpClient.PostAsync("/api/v1/business", _formData).GetAwaiter().GetResult();
        var content = _response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        Log.Information("Returned content was: {@content}", content);
    }

    private void AndUserConfirmedLegalConsentThatProvidedDataIsAccurate()
    {
        Assert.True(_request.LegalConsent);
        Assert.That(_request.LegalConsentContent, Is.EqualTo(_legalConsent));
    }
    private void AndUserUploadedAnAttachmentThatConfirmsTheExistenceOfMentionedBusiness()
    {
        Assert.True(_businessProofDocument.Length > 0);
    }

    private void AndUserUploadedAnAttachmentThatConfirmsTheUsersIdentity()
    {
        Assert.True(_userIdentityDocument.Length > 0);
    }

    private void ThenBusinessDraftRequiringVerificationIsCreatedSuccessfully()
    {
        Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    private void AndCreatorOfTheBusinessDraftCanFetchIt()
    {
        var httpClient = _app.CreateHttpClient();
        _businessDraftId = Guid.Parse(_response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
        var result = httpClient.GetAsync($"/api/v1/business/{_businessDraftId}").GetAwaiter().GetResult();
        result.EnsureSuccessStatusCode();
        var retrievedBusinessCreationDraftRequest = result.Content.ReadFromJsonAsync<FetchBusinessDraftStateResponse>().GetAwaiter().GetResult();
        BusinessDraftComparer.AreEqual(retrievedBusinessCreationDraftRequest!, _request);
    }

    private void AndInformationThatBusinessDraftWasRegisteredIsPublishedOnMessageBus()
    {
        var @event = new BusinessDraftRegisteredEvent(
            DateTime.Now,
            _businessDraftId,
            _request.UserId
            );
        A.CallTo(() => _fakeEventPublisher.SendBusinessDraftRegisteredEventAsync(A<BusinessDraftRegisteredEvent>.That.Matches(
            ev => ev.BusinessUnitId == _businessDraftId
            && ev.OwnerId == _request.UserId
            && ev.RegisteredAt.Date == DateTime.Now.Date
            ))).MustHaveHappenedOnceExactly();

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


public static class BusinessDraftComparer
{
    public static bool AreEqual(FetchBusinessDraftStateResponse response, RegisterNewBusinessRequest request)
    {
        if (response == null || request == null)
            return false;

        // Compare scalar properties
        return response.BusinessName == request.BusinessName &&
               response.BusinessType == request.BusinessType.ToString() &&  // Assuming BusinessType is an enum
               response.BusinessNIP == request.BusinessNIP &&
               response.BusinessAddress == request.BusinessAddress &&
               response.BusinessPhoneNumber == request.BusinessPhoneNumber &&
               response.BusinessEmail == request.BusinessEmail &&
               response.UserId == request.UserId &&
               response.UserFullName == request.UserFullName &&
               response.UserIdNumber == request.UserIdNumber &&
               response.UserEmail == request.UserEmail &&
               response.UserPhoneNumber == request.UserPhoneNumber &&
               response.LegalConsent == request.LegalConsent &&
               response.LegalConsentContent == request.LegalConsentContent &&
               CompareFileDocuments(response.BusinessProofDocument, request.BusinessProofDocument) &&
               CompareFileDocuments(response.UserIdentityDocument, request.UserIdentityDocument);
    }

    private static bool CompareFileDocuments(FileDocument doc1, IFormFile doc2)
    {
        if (doc1 == null && doc2 == null) return true;  // Both are null
        if (doc1 == null || doc2 == null) return false; // One is null, other is not

        var file = FileDocument.From(doc2);

        // Compare necessary properties of FileDocument and IFormFile
        return doc1.FileName == file.FileName &&
               doc1.ContentType == file.ContentType &&
               doc1.Data.Length == file.Data.Length; // You may want to compare the actual content as well
    }
}