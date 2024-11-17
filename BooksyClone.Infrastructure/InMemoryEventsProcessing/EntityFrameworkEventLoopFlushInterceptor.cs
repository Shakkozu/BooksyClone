using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace BooksyClone.Infrastructure.InMemoryEventsProcessing;

public class EntityFrameworkEventLoopFlushInterceptor : DbTransactionInterceptor
{
    private readonly EventsPublisher _eventsPublisher;

    public EntityFrameworkEventLoopFlushInterceptor(EventsPublisher eventsPublisher)
    {
        _eventsPublisher = eventsPublisher;
    }

    public override async Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData,
      CancellationToken cancellationToken = new())
    {
        await _eventsPublisher.Flush();
    }
}
