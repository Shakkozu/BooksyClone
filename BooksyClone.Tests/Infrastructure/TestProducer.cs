using BooksyClone.Infrastructure.RabbitMQStreams.Producing;

namespace BooksyClone.Tests.Infrastructure;

internal class TestProducer : RabbitMqStreamProducer
{
    public TestProducer(RabbitMQStreamProducerConfiguration config) : base(config)
    {
    }
}

