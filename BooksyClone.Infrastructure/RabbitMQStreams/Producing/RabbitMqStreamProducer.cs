using RabbitMQ.Stream.Client.Reliable;
using RabbitMQ.Stream.Client;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using Serilog;
using System.ComponentModel;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace BooksyClone.Infrastructure.RabbitMQStreams.Producing;
public record RabbitMQStreamProducerConfiguration(
    string StreamName,
    string Username,
    string Password,
    string ProducerReference,
    string RabbitMQAddress = "127.0.0.1",
    int RabbitMQPort = 5552,
    int ConnectionRetries = 3,
    TimeSpan ConnectionTimeout = default
)
{
    public RabbitMQStreamProducerConfiguration(string streamName, string address, int port)
        : this(streamName, "guest", "guest", "test-producer", address, port, 3, TimeSpan.FromSeconds(10)) { }
}


public interface IRabbitStreamProducer
{
    Task Send<T>(T message);
}
public abstract class RabbitMqStreamProducer : IRabbitStreamProducer
{
    private readonly RabbitMQStreamProducerConfiguration _config;
    private DeduplicatingProducer? _deduplicatingProducer;

    protected RabbitMqStreamProducer(RabbitMQStreamProducerConfiguration config)
    {
        if(config == null) throw new ArgumentNullException("config");
        _config = config;
        Initialize().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private async Task Initialize()
    {
        var streamSystemConfig = new StreamSystemConfig
        {
            UserName = _config.Username,
            Password = _config.Password,
            Endpoints = [new IPEndPoint(IPAddress.Parse(_config.RabbitMQAddress), _config.RabbitMQPort),]
        };
        var streamSystem = StreamSystem.Create(streamSystemConfig).ConfigureAwait(false).GetAwaiter().GetResult();
        if (!await streamSystem.StreamExists(_config.StreamName).ConfigureAwait(false))
        {
            await streamSystem.CreateStream(new StreamSpec(_config.StreamName)).ConfigureAwait(false);
        }
        var producerConfig = new DeduplicatingProducerConfig(streamSystem, _config.StreamName, _config.ProducerReference);
        _deduplicatingProducer = await DeduplicatingProducer.Create(producerConfig).ConfigureAwait(false);
    }

    public virtual async Task Send<T>(T message)
    {
        if(_deduplicatingProducer == null)
            throw new InvalidOperationException("producer is not yet initialized");

        var lastid = await _deduplicatingProducer.GetLastPublishedId().ConfigureAwait(false);
        var messageContent = JsonConvert.SerializeObject(message);
        await _deduplicatingProducer.Send(lastid + 1, new Message(Encoding.UTF8.GetBytes(messageContent)));
    }
}