using BooksyClone.Contract.BusinessOnboarding;
using BooksyClone.Infrastructure.RabbitMQStreams.Consuming;

namespace BooksyClone.Domain.Schedules.RegisteringNewBusinessUnit;

internal class NewBusinessDraftRegisteredConsumer : RabbitMQStreamsConsumer<BusinessDraftRegisteredEvent>
{
    private readonly SchedulesFacade _schedulesFacade;

    public NewBusinessDraftRegisteredConsumer(RabbitMQStreamConsumerConfiguration config,
        SchedulesFacade schedulesFacade) : base(config)
    {
        _schedulesFacade = schedulesFacade;
    }
    protected override async Task HandleAsync(BusinessDraftRegisteredEvent message)
    {
        var command = new RegisterNewBusinesUnitCommand(message.BusinessUnitId, message.OwnerId);
        await _schedulesFacade.RegisterNewBusinessUnit(command, CancellationToken.None);
    }
}
