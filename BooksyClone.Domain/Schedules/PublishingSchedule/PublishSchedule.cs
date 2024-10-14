using BooksyClone.Contract.Schedules;
using BooksyClone.Domain.Schedules.Shared;
using BooksyClone.Infrastructure.RabbitMQStreams.Producing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BooksyClone.Domain.Schedules.PublishingSchedule;
public static class PublishScheduleRoute
{
    public static string Path = "companies/{businessUnitId}/employees/{employeeId}/schedules/{yearMonth}/publish";
    public static IEndpointRouteBuilder MapPublishScheduleEndpoint(this IEndpointRouteBuilder route)
    {
        route.MapPut($"/api/v1/{Path}", async (
                HttpContext httpContext,
             Guid businessUnitId,
             Guid employeeId,
             string yearMonth,
             SchedulesFacade facade,
             CancellationToken ct) =>
        {
            var author = GetModificationAuthor(httpContext);
            await facade.PublishScheduleAsync(author, businessUnitId, employeeId, new YearMonth(yearMonth), ct);
            return Results.Ok();
        });
        return route;
    }

    private static Guid GetModificationAuthor(HttpContext httpContext)
    {
        // todo fetch author id from http context
        return Guid.NewGuid();
    }
}

public interface ISchedulesRabbitStreamsPublisher : IRabbitStreamProducer
{
    Task Send(EmployeeSchedulePublishedEvent @event);
}

internal class SchedulesPublisher : RabbitMqStreamProducer, ISchedulesRabbitStreamsPublisher
{
    public SchedulesPublisher(RabbitMQStreamProducerConfiguration config) : base(config)
    {
    }

    public async Task Send(EmployeeSchedulePublishedEvent @event)
    {
        await Send<EmployeeSchedulePublishedEvent>(@event);
    }
}