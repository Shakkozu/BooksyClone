using BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;
using BooksyClone.Domain.Storage;
using System.Runtime.CompilerServices;

namespace BooksyClone.Domain.BusinessOnboarding.Model;

public class BusinessDraft : BaseEntity
{
    public UserDetails UserDetails { get; set; }
    public BusinessDetails BusinessDetails { get; set; }
    public FileDocument BusinessProofDocument { get; internal set; }
    public FileDocument UserIdentityDocument { get; internal set; }
    public string Status { get; internal set; }
    public DateTime CreatedAt { get; internal set; }
    public DateTime UpdatedAt { get; internal set; }
    public bool LegalConsent { get; internal set; }
    public string LegalConsentContent { get; internal set; }

    public static BusinessDraft From(RegisterNewBusinessRequest contentBody)
    {
        var businessGuid = Guid.NewGuid();
        if (contentBody.LegalConsent == false)
        {
            throw new InvalidOperationException("User has to accept consent");
        }
        if (contentBody.LegalConsentContent != LegalConsents.CurrentTrueInformationConsent.Content)
        {
            throw new InvalidOperationException("Consent content accepted by user is invalid");
        }

        return new BusinessDraft
        {
            BusinessDetails = new BusinessDetails
            {
                Address = contentBody.BusinessAddress,
                Email = contentBody.BusinessEmail,
                Guid = businessGuid,
                Name = contentBody.BusinessName,
                Nip = contentBody.BusinessNIP,
                PhoneNumber = contentBody.BusinessPhoneNumber,
                Type = contentBody.BusinessType
            },
            UserDetails = new UserDetails
            {
                PhoneNumber = contentBody.UserPhoneNumber,
                Email = contentBody.UserEmail,
                FullName = contentBody.UserFullName,
                Guid = contentBody.UserId,
                IdNumber = contentBody.UserIdNumber,
            },
            UpdatedAt = DateTime.Now.ToUniversalTime(),
            CreatedAt = DateTime.Now.ToUniversalTime(),
            Guid = businessGuid,
            Status = "Draft",
            BusinessProofDocument = FileDocument.From(contentBody.BusinessProofDocument),
            UserIdentityDocument = FileDocument.From(contentBody.UserIdentityDocument),

            LegalConsent = contentBody.LegalConsent,
            LegalConsentContent = contentBody.LegalConsentContent,
        };
    }
}

internal enum BusinessDraftStatus
{
    WaitingForVerification,
    RequestedCorrections,
    Accepted
}
