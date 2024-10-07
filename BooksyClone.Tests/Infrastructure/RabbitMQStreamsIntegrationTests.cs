using BooksyClone.Infrastructure.RabbitMQStreams.Consuming;
using BooksyClone.Infrastructure.RabbitMQStreams.Producing;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Tests.Infrastructure;
[TestFixture]
internal class RabbitMQStreamsIntegrationTests
{
    private TestProducer _testProducer;
    private TestConsumer _testConsumer;
    private ITestService _fakeDependency;
    private BooksyCloneApp _app;

    private RabbitMQStreamProducerConfiguration GetTestConfiguration => new RabbitMQStreamProducerConfiguration(
        "booksy-clone-integration-tests-stream",
        "127.0.0.1",
        5552);
    private RabbitMQStreamConsumerConfiguration GetTestConsumerConfiguration => new RabbitMQStreamConsumerConfiguration(
        "booksy-clone-integration-tests-stream",
        "127.0.0.1",
        5552,
        "test-consumer");

    [SetUp]
    public void Setup()
    {
        _testProducer = new TestProducer(GetTestConfiguration);
        _fakeDependency = A.Fake<ITestService>();
        _testConsumer = new TestConsumer(_fakeDependency, GetTestConsumerConfiguration);
        _app = BooksyCloneApp.CreateInstance(services =>
        {
            services.AddSingleton(_testProducer);
            services.AddHostedService(sp =>
            {
                return _testConsumer;
            });
        });
    }

    [TearDown]
    public async Task Teardown()
    {
        _app.Dispose();
    }

    [Test]
    public async Task ConsumerShouldConsumeReceivedMessages()
    {
        var payload = new TestRabbitMqStreamsMessage("Dupa", DateTime.Now);
        await _testProducer.Send(payload);

        Task.Delay(50).Wait();

        A.CallTo(() => _fakeDependency.Call(A<TestRabbitMqStreamsMessage>.That.IsEqualTo(payload)))
            .MustHaveHappenedOnceExactly();
    }

    private class TestProducer : RabbitMqStreamProducer<TestRabbitMqStreamsMessage>
    {
        public TestProducer(RabbitMQStreamProducerConfiguration config) : base(config)
        {
        }
    }
}
public record TestRabbitMqStreamsMessage(string Message, DateTime Timestamp);

internal class TestConsumer : RabbitMQStreamsConsumer<TestRabbitMqStreamsMessage>
{
    private readonly ITestService _testService;
    private RabbitMQStreamConsumerConfiguration _config;

    public TestConsumer(ITestService testService, RabbitMQStreamConsumerConfiguration config) : base(config)
    {
        _testService = testService;
        _config = config;
    }

    protected override Task HandleAsync(TestRabbitMqStreamsMessage message)
    {
        _testService.Call(message);
        return Task.CompletedTask;
    }
}

public interface ITestService
{
    void Call(TestRabbitMqStreamsMessage message);
}

