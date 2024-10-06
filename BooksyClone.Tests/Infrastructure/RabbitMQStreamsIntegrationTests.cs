using BooksyClone.Infrastructure.RabbitMQStreams.Producing;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Tests.Infrastructure;
[TestFixture]
internal class RabbitMQStreamsIntegrationTests
{
    private TestProducer _testProducer;
    private BooksyCloneApp _app;

    private RabbitMQStreamProducerConfiguration GetTestConfiguration => new RabbitMQStreamProducerConfiguration(
        "booksy-clone-integration-tests-stream",
        "127.0.0.1",
        5552);

    [SetUp]
    public void Setup()
    {
        _testProducer = new TestProducer(GetTestConfiguration);
        _app = BooksyCloneApp.CreateInstance(services =>
        {
            services.AddSingleton(_testProducer);
        });
    }

    [TearDown]
    public async Task Teardown()
    {
        _app.Dispose();
    }

    [Test]
    public void PublishingShouldNotThrowAnyErrors()
    {
        Assert.DoesNotThrowAsync(async() => await _testProducer.Send(new TestMessage("Dupa", DateTime.Now)));
    }
    private record TestMessage(string Message, DateTime Timestamp);

    private class TestProducer : RabbitMqStreamProducer<TestMessage>
    {
        public TestProducer(RabbitMQStreamProducerConfiguration config) : base(config)
        {
        }
    }
}
