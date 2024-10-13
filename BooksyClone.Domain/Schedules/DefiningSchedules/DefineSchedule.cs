using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BooksyClone.Domain.Schedules.DefiningSchedules;

public static class DefineScheuduleRoute
{
    public static string Path = "companies/{businessUnitId}/employees/{employeeId}/schedules";
    public static IEndpointRouteBuilder MapDefineScheuduleEndpoint(this IEndpointRouteBuilder route)
    {
        route.MapPost($"/api/v1/{Path}", async (
             Guid businessUnitId,
             Guid employeeId,
             SchedulesFacade facade,
             CancellationToken ct,
             [FromBody] MonthlyScheduleDto dto) =>
        {
            await facade.DefineScheduleAsync(businessUnitId, employeeId, dto, ct);
            return Results.Ok();
        });
        return route;
    }
}


public class MonthlyScheduleDto
{
    public string ScheduleDate { get; set; }
    public MonthlyScheduleDefinitionDto[] scheduleDefinition { get; set; } = [];
}

public class MonthlyScheduleDefinitionDto
{
    public string from { get; set; }
    public string to { get; set; }
    public ShiftDto[] shifts { get; set; } = [];
    public string? description { get; set; }
}

public class ShiftDto
{
    public string start { get; set; }
    public string end { get; set; }
}
