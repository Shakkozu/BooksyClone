using BooksyClone.Domain.BusinessOnboarding.Model;
using BooksyClone.Infrastructure.Storage;
using BooksyClone.Domain.Storage;
using Microsoft.EntityFrameworkCore;
using BooksyClone.Domain.BusinessOnboarding.FetchingBusinessCreationApplication;
using BooksyClone.Contract.BusinessOnboarding;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.Configuration;

namespace BooksyClone.Domain.BusinessOnboarding;


internal class OnboardingBuilder(IConfiguration configuration, IOnboardingEventsPublisher onboardingEventsPublisher)
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IOnboardingEventsPublisher _onboardingEventsPublisher = onboardingEventsPublisher;

    internal OnboardingFacade Build()
    {
        var dbContext = new PostgresDbContext(_configuration.GetPostgresDatabaseConnectionString());
        return new OnboardingFacade(dbContext, _onboardingEventsPublisher);
    }
    
}
public  class OnboardingFacade
{
    private readonly PostgresDbContext _dbContext;
    private readonly IOnboardingEventsPublisher _onboardingEventsPublisher;

    internal OnboardingFacade(PostgresDbContext dbContext,
        IOnboardingEventsPublisher onboardingEventsPublisher)
    {
        _dbContext = dbContext;
        _onboardingEventsPublisher = onboardingEventsPublisher;
    }
    public  async Task<Guid> RegisterNewBusinessDraftAsync(BusinessDraft businessDraft,
        CancellationToken ct)
    {
        await _dbContext.AddAndSave(businessDraft, ct);
        var draftPublishedEvent = new BusinessDraftRegisteredEvent(businessDraft.CreatedAt,
            businessDraft.Guid,
            businessDraft.UserDetails.Guid);

        await _onboardingEventsPublisher.SendBusinessDraftRegisteredEventAsync(draftPublishedEvent);
        return businessDraft.Guid;
    }

    public async Task<FetchBusinessDraftStateResponse> FindById(Guid guid, CancellationToken ct)
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

    

