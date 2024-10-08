using BooksyClone.Contract.BusinessOnboarding;
using BooksyClone.Domain.Schedules.FetchingEmployeesSchedules;
using BooksyClone.Infrastructure.RabbitMQStreams.Consuming;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Domain.Schedules;
public static class SchedulesModule
{
    public static void InstallSchedulesModule(this IServiceCollection serviceProvider, IConfiguration configuration)
    {
        serviceProvider.AddTransient<SchedulesFacade>();
        serviceProvider.Configure<RabbitMQStreamConsumerConfiguration>(configuration.GetSection("Schedules:RabbitMQ:BusinessUnitsIntegration"));
        var businessDraftConsumer = GetBusinessUnitsRabbitConfig(configuration);
        serviceProvider.AddHostedService(sp =>
        {
            return new NewBusinessDraftRegisteredConsumer(businessDraftConsumer,
                sp.GetRequiredService<SchedulesFacade>());
        });

    }


    public static IEndpointRouteBuilder InstallSchedulesModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapFetchEmployeesSchedulesEndpoint();
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

}

internal class NewBusinessDraftRegisteredConsumer : RabbitMQStreamsConsumer<BusinessDraftRegisteredEvent>
{
    private readonly SchedulesFacade _schedulesFacade;

    public NewBusinessDraftRegisteredConsumer(RabbitMQStreamConsumerConfiguration config,
        SchedulesFacade schedulesFacade) : base(config)
    {
        _schedulesFacade = schedulesFacade;
    }
    protected override async Task HandleAsync(BusinessDraftRegisteredEvent message)
    {
        var command = new RegisterNewBusinesUnitCommand(message.BusinessUnitId, message.OwnerId);
        await _schedulesFacade.RegisterNewBusinessUnit(command);
    }
}


public class EmployeScheduleDto
{
    public Guid EmployeeId { get; set; }
    public string YearMonth { get; set; } //format yyyy-MM
    public IEnumerable<DailyScheduleDto> Schedule { get; set; }

}
public class MonthlyScheduleDto
{
    public IEnumerable<DailyScheduleDto> Schedule { get; set; }
}


public class DailyScheduleDto
{
    public DateOnly Date { get; set; }
    public TimeOnly Starts { get; set; }
    public TimeOnly End { get; set; }
}
