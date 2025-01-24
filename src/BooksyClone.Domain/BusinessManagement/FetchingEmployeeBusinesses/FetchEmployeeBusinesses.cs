using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using BooksyClone.Domain.Availability.Storage;
using Dapper;
using BooksyClone.Domain.Auth;

namespace BooksyClone.Domain.BusinessManagement.FetchingEmployeeBusinesses;

internal class FetchEmployeeBusinesses
{
	private readonly DbConnectionFactory _dbConnectionFactory;

	public FetchEmployeeBusinesses(DbConnectionFactory dbConnectionFactory)
	{
		_dbConnectionFactory = dbConnectionFactory;
	}

	// TODO return brief information about all businesses to which user has access as an employee
	internal async Task<IEnumerable<Guid>> HandleAsync(Guid userId, CancellationToken ct)
	{
		const string sql = @"
            SELECT business_id
            FROM business_management.employees
            WHERE user_id = @userId";
		using var connection = _dbConnectionFactory.CreateConnection();
		connection.Open();
		return await connection.QueryAsync<Guid>(sql, new { userId = userId });
	}
}

public static class Route
{
	public static IEndpointRouteBuilder MapGetEmployeeBusinessesEndpoint(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/api/employee/companies", async (HttpContext context,
			AuthFacade authFacade,
			BusinessManagementFacade facade
			) =>
		{
			var userId = authFacade.GetLoggedUserId();
			return await facade.FetchEmployeeBusinesses(userId);
		});

		return endpoints;
	}
}