using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksyClone.Infrastructure.RabbitMQStreams;
public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken ct);
}
