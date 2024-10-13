using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksyClone.Domain.Schedules.FetchingEmployeesSchedules;
internal static class FetchEmployeesSchedulesRoute
{
    internal static IEndpointRouteBuilder MapFetchEmployeesSchedulesEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/v1/companies/{companyIdentifier}/employees/schedules", async (
            HttpContext httpContext,
            SchedulesFacade schedulesFacade,
            Guid companyIdentifier) =>
        {
            return schedulesFacade.FetchCompanyEmployeesSchedules(companyIdentifier);
            

        });
        return builder;
    }
}
