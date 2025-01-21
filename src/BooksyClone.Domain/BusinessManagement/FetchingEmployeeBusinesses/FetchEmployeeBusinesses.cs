using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using BooksyClone.Domain.Auth.FetchingUserFromHttpContext;
using BooksyClone.Domain.Availability.Storage;
using BooksyClone.Domain.BusinessOnboarding.Model;

namespace BooksyClone.Domain.BusinessManagement.FetchingEmployeeBusinesses;

internal class FetchEmployeeBusinesses
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public FetchEmployeeBusinesses(DbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    // TODO return brief information about all businesses to which user has access as an employee
    internal async Task<IEnumerable<Guid>> HandleAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

public static class Route
{
    public static IEndpointRouteBuilder MapGetEmployeeBusinessesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/businesses/employee", async (HttpContext context,
            IFetchUserIdentifierFromContext fetchUserIdentifierFromContext,
            BusinessManagementFacade facade
            ) =>
        {
            await context.Response.WriteAsJsonAsync(new { Message = "Hello from the employee businesses endpoint" });
        });

        return endpoints;
    }
}