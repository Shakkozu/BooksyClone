
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Infrastructure.EventProcessing;


public interface IEventPublisher
{
    Task PublishAsync(INotification @event, CancellationToken ct);
}

public class InMemoryEventPublisher: IEventPublisher
{
    private readonly IMediator _mediator;

    public InMemoryEventPublisher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task PublishAsync(INotification @event, CancellationToken ct)
    {
        await _mediator.Publish(@event, ct);
    }
}

public interface IEventHandler<TEvent>
{
    Task HandleAsync(TEvent @event, CancellationToken ct);
}

public abstract class InMemoryEventHandler<T> : IEventHandler<T> where T : class, INotification
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(T @event, CancellationToken ct)
    {
        var handler = _serviceProvider.GetService<INotificationHandler<T>>();
        if (handler == null)
            throw new ArgumentNullException($"Initializing notification {@event.GetType().Name} handler failed due to registration issues", nameof(handler));
        await handler.Handle(@event, ct);
    }
}

