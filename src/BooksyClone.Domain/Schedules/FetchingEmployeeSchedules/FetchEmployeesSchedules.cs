using BooksyClone.Contract.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BooksyClone.Domain.Schedules.FetchingEmployeesSchedules;
internal static class FetchEmployeesSchedulesRoute
{
    internal static IEndpointRouteBuilder MapFetchEmployeesSchedulesEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/v1/companies/{companyIdentifier}/employees/schedules", async (
            HttpContext httpContext,
            SchedulesFacade schedulesFacade,
            CancellationToken ct,
            Guid companyIdentifier,
            int page = 1,
            int pageSize = 100) =>
        {
            return await schedulesFacade.FetchCompanyEmployeesSchedules(companyIdentifier, new Paging(page, pageSize), ct);
        });
        return builder;
    }
}
