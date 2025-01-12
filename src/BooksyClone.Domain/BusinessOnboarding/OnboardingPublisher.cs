using BooksyClone.Contract.BusinessOnboarding;
using BooksyClone.Infrastructure.RabbitMQStreams.Producing;

namespace BooksyClone.Domain.BusinessOnboarding;
public interface IOnboardingEventsPublisher
{
    Task SendBusinessDraftRegisteredEventAsync(BusinessDraftRegisteredEvent @event);
}
internal class OnboardingRabbitStreamsEventsPublisher : RabbitMqStreamProducer, IOnboardingEventsPublisher
{
    public OnboardingRabbitStreamsEventsPublisher(RabbitMQStreamProducerConfiguration config) : base(config)
    {
    }

    public async Task SendBusinessDraftRegisteredEventAsync(BusinessDraftRegisteredEvent @event) => await Send(@event);
}
