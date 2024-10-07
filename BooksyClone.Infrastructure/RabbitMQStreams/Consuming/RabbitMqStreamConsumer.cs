using RabbitMQ.Stream.Client.Reliable;
using RabbitMQ.Stream.Client;
using System.Text;
using Microsoft.Extensions.Hosting;
using System.Net;
using Newtonsoft.Json;
using Serilog;

namespace BooksyClone.Infrastructure.RabbitMQStreams.Consuming;

public record RabbitMQStreamConsumerConfiguration(
    string StreamName,
    string Username,
    string Password,
    string ConsumerName,
    string RabbitMQAddress = "127.0.0.1",
    int RabbitMQPort = 5552,
    int ConnectionRetries = 3,
    bool ShouldCreateStreamIfNotExists = true,
    TimeSpan ConnectionTimeout = default
)
{
    public RabbitMQStreamConsumerConfiguration(string streamName, string address, int port, string consumerName)
        : this(streamName, "guest", "guest", consumerName, address, port, 3, true, TimeSpan.FromSeconds(10)) { }
}

public abstract class RabbitMQStreamsConsumer<T>: IHostedService where T: class
{
    private readonly RabbitMQStreamConsumerConfiguration _config;
    private Consumer? _consumer;
    private StreamSystem? _streamSystem;
    private int _consumed;

    protected RabbitMQStreamsConsumer(RabbitMQStreamConsumerConfiguration config)
    {
        _config = config;
    }

    protected abstract Task HandleAsync(T message);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _consumed = 0;
        var streamSystemConfig = new StreamSystemConfig
        {
            UserName = _config.Username,
            Password = _config.Password,
            Endpoints = [new IPEndPoint(IPAddress.Parse(_config.RabbitMQAddress), _config.RabbitMQPort),]
        };
        _streamSystem = await StreamSystem.Create(streamSystemConfig);
        if (!await _streamSystem.StreamExists(_config.StreamName))
        {
            await _streamSystem.CreateStream(new StreamSpec(_config.StreamName)
            {
                MaxLengthBytes = 5_000_000_000
            });
        }

        var offset = await _streamSystem.QueryOffset(_config.ConsumerName, _config.StreamName).ConfigureAwait(false);
        var consumerConfig = new ConsumerConfig(_streamSystem, _config.StreamName)
        {
            Reference = _config.ConsumerName,
            OffsetSpec = new OffsetTypeOffset(offset),
            MessageHandler = async (stream, consumer, messageContext, message) =>
            {
                var messageStringContent = Encoding.UTF8.GetString(message.Data.Contents);
                var serializedMessage = JsonConvert.DeserializeObject<T>(messageStringContent);
                if(serializedMessage == null)
                    throw new InvalidCastException($"Deserialization of message {messageStringContent} failed casting to an object. Stream: {stream}, Index: {messageContext.Offset}, Timestamp: {messageContext.Timestamp}");

                try
                {
                    await HandleAsync(serializedMessage).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Log.Error("Processing received message failed with error: {@error}. Stream: {stream}, offset: {offset}", e, stream, messageContext.Offset);
                }
                if(_consumed++ % 1000 == 0)
                    await consumer.StoreOffset(messageContext.Offset).ConfigureAwait(false);

            }
        };
        Log.Information("Starting consumer for stream {StreamName}", _config.StreamName);
        _consumer = await Consumer.Create(consumerConfig);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if(_consumer != null)
        {
            await _consumer.Close().ConfigureAwait(false);
        }
        if(_streamSystem != null)
        {
            await _streamSystem.Close().ConfigureAwait(false);

        }
    }
}