
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Infrastructure.EventProcessing;

public interface IEventHandler<TEvent>
{
    Task HandleAsync(TEvent @event, CancellationToken ct);
}



public abstract class InMemoryEventDispatcher<T> : IEventHandler<T> where T : class, INotification
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryEventDispatcher(IServiceProvider serviceProvider)
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

