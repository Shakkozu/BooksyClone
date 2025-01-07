using BooksyClone.Infrastructure.RabbitMQStreams.Consuming;
using BooksyClone.Infrastructure.RabbitMQStreams.Producing;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BooksyClone.Tests.Infrastructure;
[TestFixture]
internal class RabbitMQStreamsIntegrationTests
{
    private RabbitMQStreamProducerConfiguration _testProducerConfiguration;
    private RabbitMQStreamConsumerConfiguration _testConsumerConfiguration;

    [SetUp]
    public void Setup()
    {
        var streamName = Guid.NewGuid().ToString();
        _testProducerConfiguration = new RabbitMQStreamProducerConfiguration(
        streamName,
        "127.0.0.1",
        5552);
        _testConsumerConfiguration = new RabbitMQStreamConsumerConfiguration(
            streamName,
            "127.0.0.1",
            5552,
            "test-consumer",
            new RetryPolicy(3, retryAttempts => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempts))));
    }

    [Test]
    public async Task ConsumerShouldConsumeReceivedMessages()
    {
        var testProducer = new TestProducer(_testProducerConfiguration);
        var fakeDependency = A.Fake<ITestService>();
        var testConsumer = new TestConsumer(fakeDependency, _testConsumerConfiguration);
        var app = BooksyCloneApp.CreateInstance(services =>
        {
            services.AddSingleton(testProducer);
            services.AddHostedService(sp =>
            {
                return testConsumer;
            });
        });
        var payload = new TestRabbitMqStreamsMessage("Dupa", DateTime.Now);
        await testProducer.Send(payload);

        Task.Delay(50).Wait();

        A.CallTo(() => fakeDependency.Call(A<TestRabbitMqStreamsMessage>.That.IsEqualTo(payload)))
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task ConsumerShouldRetrieveMessagesWhichWereProducerWhileConsumerWasOffline()
	{
		var testProducer = new TestProducer(_testProducerConfiguration);
		var fakeDependency = A.Fake<ITestService>();
		var hostedServiceTest = new HostedServiceTest(new TestConsumer(fakeDependency, _testConsumerConfiguration));
		BooksyCloneApp.CreateInstance(services =>
		{
			services.AddSingleton(testProducer);
			services.AddHostedService(sp =>
			{
				return hostedServiceTest;
			});
		});
		var payload = new TestRabbitMqStreamsMessage("Dupa", DateTime.Now);
		await testProducer.Send(payload);

		Task.Delay(50).Wait();
		A.CallTo(() => fakeDependency.Call(A<TestRabbitMqStreamsMessage>.That.IsEqualTo(payload)))
			.MustNotHaveHappened();

		await hostedServiceTest.SwitchToConsumerMode(CancellationToken.None);
		Task.Delay(50).Wait();
		A.CallTo(() => fakeDependency.Call(A<TestRabbitMqStreamsMessage>.That.IsEqualTo(payload)))
			.MustHaveHappenedOnceExactly();
	}

	[Test]
    public async Task ConsumerShouldRetryHandlingMessagesWhichCausesExceptions()
    {
        var testProducer = new TestProducer(_testProducerConfiguration);
        var fakeDependency = A.Fake<ITestService>();
        var testConsumer = new TestConsumerWhichRequiresMultipleTimestoProcessMessage(fakeDependency, _testConsumerConfiguration);
		BooksyCloneApp.CreateInstance(services =>
		{
			services.AddSingleton(testProducer);
			services.AddHostedService(sp =>
			{
				return testConsumer;
			});
		});
		var payload = new TestRabbitMqStreamsMessage("Dupa", DateTime.Now);
        await testProducer.Send(payload);

        Task.Delay(50).Wait();

        A.CallTo(() => fakeDependency.Call(A<TestRabbitMqStreamsMessage>.That.IsEqualTo(payload)))
            .MustHaveHappenedOnceExactly();
    }
}
public record TestRabbitMqStreamsMessage(string Message, DateTime Timestamp);


internal class HostedServiceTest : IHostedService
{
	private readonly TestConsumer _testConsumer;

	public HostedServiceTest(TestConsumer testConsumer)
    {
		_testConsumer = testConsumer;
	}
    internal async Task SwitchToConsumerMode(CancellationToken ct)
	{
		await _testConsumer.StartAsync(ct);
	}
	
	internal async Task StopConsumerMode(CancellationToken ct)
	{
		await _testConsumer.StopAsync(ct);
	}
	public Task StartAsync(CancellationToken cancellationToken)
	{
        Console.WriteLine("Start");
		return Task.CompletedTask;
    }

	public Task StopAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine("Stop");
		return Task.CompletedTask;
	}
}
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

internal class TestConsumerWhichRequiresMultipleTimestoProcessMessage : RabbitMQStreamsConsumer<TestRabbitMqStreamsMessage>
{
    private readonly ITestService _testService;
    private RabbitMQStreamConsumerConfiguration _config;
    private int _counter = 0;

    public TestConsumerWhichRequiresMultipleTimestoProcessMessage(ITestService testService, RabbitMQStreamConsumerConfiguration config) : base(config)
    {
        _testService = testService;
        _config = config;
    }

    protected override Task HandleAsync(TestRabbitMqStreamsMessage message)
    {
        _counter++;
        if (_counter < 3)
            throw new InvalidOperationException("retry tests");

        _testService.Call(message);
        return Task.CompletedTask;
    }
}

public interface ITestService
{
    void Call(TestRabbitMqStreamsMessage message);
}

