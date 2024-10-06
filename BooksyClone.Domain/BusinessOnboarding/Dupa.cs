using BooksyClone.Domain.BusinessOnboarding.FetchingBusinessCreationApplication;
using BooksyClone.Domain.BusinessOnboarding.Model;
using BooksyClone.Infrastructure.EventProcessing;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace BooksyClone.Domain.BusinessOnboarding;

internal interface IEvent
{
    DateTime Timestamp { get; }
}
internal interface IOnboardingNewBusinessEvent : IEvent, INotification
{
    DateTime Timestamp { get; }
    Guid BusinessDraftId { get; }
}
public class OnboardingNewBusinessEvents
{
    internal record AppliedFormtoRegisterNewBusiness(Guid BusinessDraftId, BusinessDraft BusinessDraftForm, DateTime Timestamp) : IOnboardingNewBusinessEvent;

    internal record RequestedAdditionalInformationFromUser(Guid BusinessDraftId, Guid AnalyticsId, string Message, DateTime Timestamp) : IOnboardingNewBusinessEvent;

    internal record UserProvidedAdditionalInformation(Guid BusinessDraftId, BusinessDraft BusinessDraft, DateTime Timestamp) : IOnboardingNewBusinessEvent;

    internal record ConfirmedThatProvidedDataIsCorrect(Guid BusinessDraftId, Guid AnalyticsId, DateTime Timestamp) : IOnboardingNewBusinessEvent;
}

internal class BusinessDraftRegistrationFormAggregate
{
    public BusinessDraftRegistrationFormAggregate(IEnumerable<IOnboardingNewBusinessEvent> events)
    {
        foreach (var @event in events)
            Evolve(@event);
    }

    private void Evolve(IOnboardingNewBusinessEvent @event)
    {
        if (@event is OnboardingNewBusinessEvents.AppliedFormtoRegisterNewBusiness registeredEvent)
        {
            _draftForm = registeredEvent.BusinessDraftForm;
        }
        if (@event is OnboardingNewBusinessEvents.RequestedAdditionalInformationFromUser)
        {
            _correctionsRequested = true;
        }
        if (@event is OnboardingNewBusinessEvents.UserProvidedAdditionalInformation providedAdditionalINformationsEvent)
        {
            _correctionsRequested = false;
            _corrections.Add(providedAdditionalINformationsEvent.BusinessDraft);

        }
        if (@event is OnboardingNewBusinessEvents.ConfirmedThatProvidedDataIsCorrect)
        {
            _approved = true;
        }
        throw new InvalidCastException("not known event type");
    }

    private BusinessDraft _draftForm;
    private List<BusinessDraft> _corrections = new List<BusinessDraft>();
    private bool _approved = false;
    private bool _correctionsRequested = false;

    internal IOnboardingNewBusinessEvent Register(BusinessDraft draft)
    {
        if (_draftForm != null)
            throw new InvalidOperationException("already registered");

        return new OnboardingNewBusinessEvents.AppliedFormtoRegisterNewBusiness(draft.BusinessDetails.Guid, draft, DateTime.Now);
    }
    internal IOnboardingNewBusinessEvent RequestAdditionalInformations(Guid analyticId, Guid businessDraftId, string message)
    {
        if (_draftForm == null)
            throw new InvalidOperationException("not registered");
        if (_approved)
            throw new InvalidOperationException("already approved");

        return new OnboardingNewBusinessEvents.RequestedAdditionalInformationFromUser(businessDraftId, analyticId, message, DateTime.Now);
    }

    internal IOnboardingNewBusinessEvent ApplyCorrections(BusinessDraft draft)
    {
        if (_draftForm == null)
            throw new InvalidOperationException("not registered");
        if (_correctionsRequested == false)
            throw new InvalidOperationException("user was not asked to provide additional informations");
        if (_approved)
            throw new InvalidOperationException("already approved, cannot modify");

        return new OnboardingNewBusinessEvents.UserProvidedAdditionalInformation(draft.BusinessDetails.Guid, draft, DateTime.Now);
    }

    internal IOnboardingNewBusinessEvent ApproveBusinessDraft(Guid approvedBy, Guid businessDraftId)
    {
        if (_draftForm == null)
            throw new InvalidOperationException("not registered");
        if (_approved)
            throw new InvalidOperationException("already approved");

        return new OnboardingNewBusinessEvents.ConfirmedThatProvidedDataIsCorrect(businessDraftId, approvedBy, DateTime.Now);
    }
}


internal class OnboardingEventsStore
{
    public OnboardingEventsStore(IMediator mediator)
    {
        _mediator = mediator;
    }
    private Dictionary<Guid, IList<IOnboardingNewBusinessEvent>> _cache = new();
    private IMediator _mediator;

    internal void Save(IOnboardingNewBusinessEvent @event)
    {
        _mediator.Publish(@event, CancellationToken.None).GetAwaiter().GetResult();
        if (!_cache.ContainsKey(@event.BusinessDraftId))
        {
            _cache[@event.BusinessDraftId] = new List<IOnboardingNewBusinessEvent> { @event };
            return;
        }

        if (_cache[@event.BusinessDraftId].Any(x => x.Timestamp == @event.Timestamp))
            return;

    }

    public IEnumerable<IOnboardingNewBusinessEvent> GetEventsStream(Guid businessUnitId)
    {
        return !_cache.ContainsKey(businessUnitId)
            ? []
            : (IEnumerable<IOnboardingNewBusinessEvent>)new List<IOnboardingNewBusinessEvent>(_cache[businessUnitId]);
    }

    public IEnumerable<IOnboardingNewBusinessEvent> ReadStreamUntil(Guid businessUnitId, DateTime dt)
    {
        return !_cache.ContainsKey(businessUnitId)
            ? []
            : new List<IOnboardingNewBusinessEvent>(_cache[businessUnitId].Where(x => x.Timestamp < dt));
    }
}


public class FormsRequiringVerificationReadModel
{
    public Guid BusinessDraftId { get; set; }
    public string BusinessName { get; set; }
    public DateTime RequestedAt { get; set; }
    public string RequestMessage { get; set; }
    public Guid AnalystId { get; set; }
}

public class CurrentBusinessDraftFormReadModel
{
    public Guid BusinessDraftId { get; set; }
    public FetchBusinessDraftStateResponse DraftState { get; set; }
    public bool IsApproved { get; set; }
    public bool AdditionalInformationRequired { get; set; }
    public DateTime LastUpdated { get; set; }
}

internal class CurrentBusinessDraftFormProjection : InMemoryEventHandler<IOnboardingNewBusinessEvent>
{
    private Dictionary<Guid, CurrentBusinessDraftFormReadModel> _cache = new();
    public CurrentBusinessDraftFormProjection(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public Task Handle(IOnboardingNewBusinessEvent notification, CancellationToken cancellationToken)
    {
        Apply(notification);
        return Task.CompletedTask;
    }

    internal CurrentBusinessDraftFormReadModel GetReadModel(Guid businessDraftId)
    {
        return _cache[businessDraftId];
    }

    private void Apply(IOnboardingNewBusinessEvent @event)
    {
        if (@event is OnboardingNewBusinessEvents.AppliedFormtoRegisterNewBusiness appliedForm)
        {
            _cache[@event.BusinessDraftId] = new CurrentBusinessDraftFormReadModel
            {
                BusinessDraftId = appliedForm.BusinessDraftId,
                DraftState = FromBusinessDraft(appliedForm.BusinessDraftForm),
                IsApproved = false,
                LastUpdated = appliedForm.Timestamp
            };
        }
        if (@event is OnboardingNewBusinessEvents.RequestedAdditionalInformationFromUser requestedAdditionalInformation)
        {
            _cache[@event.BusinessDraftId].AdditionalInformationRequired = true;
            _cache[@event.BusinessDraftId].LastUpdated = requestedAdditionalInformation.Timestamp;
        }
        if (@event is OnboardingNewBusinessEvents.UserProvidedAdditionalInformation formUpdated)
        {
            _cache[@event.BusinessDraftId].DraftState = FromBusinessDraft(formUpdated.BusinessDraft);
            _cache[@event.BusinessDraftId].LastUpdated = formUpdated.Timestamp;
            _cache[@event.BusinessDraftId].AdditionalInformationRequired = false;

        }
        if (@event is OnboardingNewBusinessEvents.ConfirmedThatProvidedDataIsCorrect confirmed)
        {
            _cache[@event.BusinessDraftId].LastUpdated = confirmed.Timestamp;
            _cache[@event.BusinessDraftId].IsApproved = true;
        }
    }

    private FetchBusinessDraftStateResponse FromBusinessDraft(BusinessDraft draft)
    {
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

/*
 * 
 * public class OnboardingNewBusinessEvents
{
    internal record AppliedFormtoRegisterNewBusiness(Guid BusinessDraftId, BusinessDraft BusinessDraftForm, DateTime Timestamp) : IOnboardingNewBusinessEvent;

    internal record RequestedAdditionalInformationFromUser(Guid BusinessDraftId, Guid AnalyticsId, string Message, DateTime Timestamp) : IOnboardingNewBusinessEvent;

    internal record UserProvidedAdditionalInformation(Guid BusinessDraftId, BusinessDraft BusinessDraft, DateTime Timestamp) : IOnboardingNewBusinessEvent;

    internal record ConfirmedThatProvidedDataIsCorrect(Guid BusinessDraftId, Guid AnalyticsId, DateTime Timestamp) : IOnboardingNewBusinessEvent;
}
 */