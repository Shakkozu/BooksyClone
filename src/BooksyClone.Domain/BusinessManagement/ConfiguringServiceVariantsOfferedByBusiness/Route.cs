using BooksyClone.Contract.BusinessManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BooksyClone.Domain.BusinessManagement.ConfiguringServiceVariantsOfferedByBusiness;

internal static class Route
{
    internal static IEndpointRouteBuilder MapConfigureServiceVariantsOfferedByBusinessEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/v1/companies/{businessUnitId}/services-configuration", async (HttpContext context,
            BusinessManagementFacade facade,
            CancellationToken ct,
            [FromBody] BusinessConfigurationDto dto) =>
        {
            await facade.ConfigureServicesOfferedByBusiness(dto, ct);
            return Results.NoContent();
        });

        return endpoints;
    }
}