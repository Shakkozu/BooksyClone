using BooksyClone.Domain.BusinessOnboarding.Model;
using BooksyClone.Infrastructure.Storage;
using BooksyClone.Domain.Storage;
using Microsoft.EntityFrameworkCore;
using BooksyClone.Domain.BusinessOnboarding.FetchingBusinessCreationApplication;

namespace BooksyClone.Domain.BusinessOnboarding;

internal class OnboardingFacade
{
    private readonly SqliteDbContext _dbContext;

    public OnboardingFacade(SqliteDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    internal async Task<Guid> RegisterNewBusinessDraftAsync(BusinessDraft businessDraft, CancellationToken ct)
    {
        await _dbContext.AddAndSave(businessDraft, ct);
        return businessDraft.Guid;
    }

    internal async Task<FetchBusinessDraftStateResponse> FindById(Guid guid, CancellationToken ct)
    {
        var draft = await _dbContext.BusinessDrafts.AsQueryable().SingleAsync(x => x.Guid == guid, ct);
        if (draft == null) throw new ArgumentNullException(nameof(draft));

        return new FetchBusinessDraftStateResponse
        {
            BusinessName = draft.BusinessDetails.Name,
            BusinessType = draft.BusinessDetails.Type.ToString(),
            BusinessNIP = draft.BusinessDetails.Nip,
            BusinessAddress = draft.BusinessDetails.Address,
            BusinessPhoneNumber = draft.BusinessDetails.PhoneNumber,
            BusinessEmail = draft.BusinessDetails.Email,

            UserId = draft.UserDetails.Guid,
            UserFullName = draft.UserDetails.FullName,
            UserIdNumber = draft.UserDetails.IdNumber,
            UserEmail = draft.UserDetails.Email,
            UserPhoneNumber = draft.UserDetails.PhoneNumber,

            BusinessProofDocument = draft.BusinessProofDocument,
            UserIdentityDocument = draft.UserIdentityDocument,

            LegalConsent = draft.LegalConsent,
            LegalConsentContent = draft.LegalConsentContent
        };
    }
}

    

