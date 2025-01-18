using Bogus;
using Bogus.Extensions.Poland;
using BooksyClone.Domain.BusinessOnboarding;
using BooksyClone.Domain.BusinessOnboarding.Model;
using BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;
using Microsoft.AspNetCore.Http;

namespace BooksyClone.Tests.BusinessOnboarding;

internal class OnboardingFixture(OnboardingFacade onboardingFacade)
{
    internal async Task<Guid> ABusinessExists(Guid userId, RegisterNewBusinessRequest? request = null)
    {
        request ??= ACreateNewBusinessDraftRequest(userId);
        request.UserId = userId;
        
        return await onboardingFacade.RegisterNewBusinessDraftAsync(BusinessDraft.From(request), CancellationToken.None);
    }
    
    
    internal RegisterNewBusinessRequest ACreateNewBusinessDraftRequest(Guid userId)
    {
        const string legalConsent = "Oświadczam że wprowadzone przeze mnie dane są poprawne i zgodne z stanem faktycznym.";
        var faker = new Faker("pl");
        var businessProofDocument = CreateFakeFormFile("businessIdentificationDocument.jpg", "test");
        var userIdentityDocument = CreateFakeFormFile("userIdentity.jpg", "test");
        var request = new RegisterNewBusinessRequest
        {
            BusinessAddress = faker.Address.FullAddress(),
            BusinessName = faker.Company.CompanyName(),
            BusinessType = BusinessType.Barber,
            BusinessEmail = faker.Internet.Email(),
            BusinessNIP = "7755446779",
            CorrelationId = Guid.NewGuid(),
            Timestamp = DateTime.Today.AddHours(15).ToUniversalTime(),
            UserEmail = faker.Person.Email,
            BusinessPhoneNumber = faker.Phone.PhoneNumber("#########"),
            UserFullName = faker.Person.FullName,
            UserIdNumber = faker.Person.Pesel(),
            UserId = userId,
            UserPhoneNumber = faker.Phone.PhoneNumber("#########"),
            LegalConsent = true,
            LegalConsentContent = legalConsent,
            BusinessProofDocument = businessProofDocument,
            UserIdentityDocument = userIdentityDocument
        };
        
        return request;
    }
    
    private IFormFile CreateFakeFormFile(string fileName, string content)
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