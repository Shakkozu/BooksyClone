using BooksyClone.Domain.Schedules.DefiningSchedules;
using BooksyClone.Domain.Schedules.FetchingEmployeeScheduleDetails;
using BooksyClone.Domain.Schedules.FetchingEmployeesSchedules;
using BooksyClone.Domain.Schedules.PublishingSchedule;
using BooksyClone.Domain.Schedules.RegisteringNewBusinessUnit;
using BooksyClone.Domain.Schedules.Storage;
using BooksyClone.Infrastructure.RabbitMQStreams.Consuming;
using BooksyClone.Infrastructure.RabbitMQStreams.Producing;
using BooksyClone.Infrastructure.TimeManagement;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata.Ecma335;

namespace BooksyClone.Domain.Schedules;
public static class SchedulesModule
{
    public static void InstallSchedulesModule(this IServiceCollection serviceProvider, IConfiguration configuration)
    {
        serviceProvider.AddSingleton<ITimeService, TimeService>();
        serviceProvider.AddTransient<SchedulesBuilder>();
        serviceProvider.AddTransient<SchedulesFacade>(sp =>
        {
            var builder = sp.GetRequiredService<SchedulesBuilder>();
            return builder.GetFacade();
        });
        var businessDraftConsumer = GetBusinessUnitsRabbitConfig(configuration);
        serviceProvider.AddHostedService(sp =>
        {
            return new NewBusinessDraftRegisteredConsumer(businessDraftConsumer,
                sp.GetRequiredService<SchedulesFacade>());
        });
        serviceProvider.AddTransient<IScheduleDefinitionRepository, EntityFrameworkScheduleDefinitionRepository>();

        serviceProvider.AddSingleton<ISchedulesPublisher, SchedulesPublisher>(_ => new SchedulesPublisher(GetSchedulesProducerConfiguration(configuration)));

    }


    public static IEndpointRouteBuilder InstallSchedulesModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapFetchEmployeesSchedulesEndpoint();
        endpoints.MapDefineScheuduleEndpoint();
        endpoints.MapFetchScheduleDetailsEndpoint();
        endpoints.MapPublishScheduleEndpoint();
        return endpoints;

    }

    private static RabbitMQStreamConsumerConfiguration GetBusinessUnitsRabbitConfig(IConfiguration configuration)
    {
        var streamName = configuration.GetValue<string>("Schedules:RabbitMQ:BusinessUnitsIntegration:StreamName");
        var username = configuration.GetValue<string>("Schedules:RabbitMQ:BusinessUnitsIntegration:Username");
        var password = configuration.GetValue<string>("Schedules:RabbitMQ:BusinessUnitsIntegration:Password");
        var consumername = configuration.GetValue<string>("Schedules:RabbitMQ:BusinessUnitsIntegration:ConsumerName");
        var rabbitmqAddress = configuration.GetValue<string>("Schedules:RabbitMQ:BusinessUnitsIntegration:RabbitMQAddress");
        var rabbitmqPort = int.Parse(configuration.GetValue<string>("Schedules:RabbitMQ:BusinessUnitsIntegration:RabbitMQPort") ?? throw new ArgumentException());
        var businessDraftConsumer = new RabbitMQStreamConsumerConfiguration
        (
            streamName ?? throw new ArgumentNullException(streamName),
            username ?? throw new ArgumentNullException(username),
            password ?? throw new ArgumentNullException(password),
            consumername ?? throw new ArgumentNullException(consumername),
            RetryPolicy.Default,
            RabbitMQAddress: rabbitmqAddress ?? throw new ArgumentNullException(rabbitmqAddress),
            RabbitMQPort: rabbitmqPort
            );
        return businessDraftConsumer;
    }
    
    private static RabbitMQStreamProducerConfiguration GetSchedulesProducerConfiguration(IConfiguration configuration)
    {
        var streamName = configuration.GetValue<string>("Schedules:RabbitMQ:SchedulesProducer:StreamName");
        var username = configuration.GetValue<string>("Schedules:RabbitMQ:SchedulesProducer:Username");
        var password = configuration.GetValue<string>("Schedules:RabbitMQ:SchedulesProducer:Password");
        var produerRef = configuration.GetValue<string>("Schedules:RabbitMQ:SchedulesProducer:ProducerReferece");
        var rabbitmqAddress = configuration.GetValue<string>("Schedules:RabbitMQ:SchedulesProducer:RabbitMQAddress");
        var rabbitmqPort = int.Parse(configuration.GetValue<string>("Schedules:RabbitMQ:SchedulesProducer:RabbitMQPort") ?? throw new ArgumentException());
        var businessDraftConsumer = new RabbitMQStreamProducerConfiguration
        (
            streamName ?? throw new ArgumentNullException(streamName),
            username ?? throw new ArgumentNullException(username),
            password ?? throw new ArgumentNullException(password),
            produerRef ?? throw new ArgumentNullException(produerRef),
            rabbitmqAddress ?? throw new ArgumentNullException(rabbitmqAddress),
            rabbitmqPort
            );
        return businessDraftConsumer;
    }

}
