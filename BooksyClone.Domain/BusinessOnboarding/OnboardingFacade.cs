using BooksyClone.Domain.BusinessOnboarding.Model;
using BooksyClone.Infrastructure.Storage;
using BooksyClone.Domain.Storage;
using Microsoft.EntityFrameworkCore;
using BooksyClone.Domain.BusinessOnboarding.FetchingBusinessCreationApplication;
using BooksyClone.Infrastructure.EventProcessing;
using BooksyClone.Contract.BusinessOnboarding;

namespace BooksyClone.Domain.BusinessOnboarding;

internal class OnboardingFacade
{
    private readonly SqliteDbContext _dbContext;
    private readonly IEventPublisher _eventPublisher;
    private readonly CurrentBusinessDraftFormProjection _currentBusinessDraftFormProjection;
    private readonly OnboardingEventsStore _onboardingEventsStore;

    public OnboardingFacade(SqliteDbContext dbContext,
        IEventPublisher eventPublisher,
        CurrentBusinessDraftFormProjection currentBusinessDraftFormProjection,
        OnboardingEventsStore onboardingEventsStore)
    {
        _dbContext = dbContext;
        _eventPublisher = eventPublisher;
        _currentBusinessDraftFormProjection = currentBusinessDraftFormProjection;
        _onboardingEventsStore = onboardingEventsStore;
    }
    internal async Task<Guid> RegisterNewBusinessDraftAsync(BusinessDraft businessDraft,
        CancellationToken ct)
    {
        return await RegisterEvents(businessDraft, ct);


        await _dbContext.AddAndSave(businessDraft, ct);
        var draftPublishedEvent = new BusinessDraftRegisteredEvent(businessDraft.CreatedAt,
            businessDraft.Guid,
            businessDraft.UserDetails.Guid);

        await _eventPublisher.PublishAsync(draftPublishedEvent, ct);
        return businessDraft.Guid;
    }
    internal async Task<Guid> RegisterEvents(BusinessDraft businessDraft,
        CancellationToken ct)
    {
        var events = _onboardingEventsStore.GetEventsStream(businessDraft.Guid) ?? [];
        var aggregate = new BusinessDraftRegistrationFormAggregate(events);
        var result = aggregate.Register(businessDraft);
        _onboardingEventsStore.Save(result);

        var draftPublishedEvent = new BusinessDraftRegisteredEvent(businessDraft.CreatedAt,
            businessDraft.Guid,
            businessDraft.UserDetails.Guid);

        await _eventPublisher.PublishAsync(draftPublishedEvent, ct);

        return businessDraft.Guid;
    }

    internal async Task<FetchBusinessDraftStateResponse> FindById(Guid guid, CancellationToken ct)
    {
        return await FindByIdEvents(guid, ct);
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

    internal async Task<FetchBusinessDraftStateResponse> FindByIdEvents(Guid guid, CancellationToken ct)
    {
        var events = _onboardingEventsStore.GetEventsStream(guid);
        return _currentBusinessDraftFormProjection.GetReadModel(guid).DraftState;
    }

}

    

