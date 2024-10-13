using BooksyClone.Contract.Schedules;
using BooksyClone.Domain.Schedules.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace BooksyClone.Domain.Schedules.FetchingEmployeeScheduleDetails;
public static class FetchScheuduleDetailsRoute
{
    public static string Path = "companies/{businessUnitId}/employees/{employeeId}/schedules/{scheduleDate}";
    public static IEndpointRouteBuilder MapFetchScheduleDetailsEndpoint(this IEndpointRouteBuilder route)
    {
        route.MapGet($"/api/v1/{Path}", async (
             Guid businessUnitId,
             Guid employeeId,
             string scheduleDate,
             CancellationToken ct,
             SchedulesFacade facade) =>
        {
            return await facade.FetchEmployeeScheduleDetailsAsync(businessUnitId, employeeId, new YearMonth(scheduleDate), ct);


        });
        return route;
    }
}


public class FetchScheduleDefinitionDetailsResponse
{
    public string Status { get; set; }
    public string ScheduleData { get; set; }
    public IEnumerable<MonthlyScheduleDefinitionDto> ScheduleDefinition { get; set; }
}