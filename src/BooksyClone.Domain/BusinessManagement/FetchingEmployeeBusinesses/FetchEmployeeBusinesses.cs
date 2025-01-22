using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using BooksyClone.Domain.Availability.Storage;
using Dapper;

namespace BooksyClone.Domain.BusinessManagement.FetchingEmployeeBusinesses;

internal class FetchEmployeeBusinesses
{
	private readonly DbConnectionFactory _dbConnectionFactory;

	public FetchEmployeeBusinesses(DbConnectionFactory dbConnectionFactory)
	{
		_dbConnectionFactory = dbConnectionFactory;
	}

	// TODO return brief information about all businesses to which user has access as an employee
	internal async Task<IEnumerable<Guid>> HandleAsync(Guid employeeId, CancellationToken ct)
	{
		const string sql = @"
            SELECT business_id
            FROM business_management.employees
            WHERE employee_id = @employee_id";
		using var connection = _dbConnectionFactory.CreateConnection();
		connection.Open();
		return await connection.QueryAsync<Guid>(sql, new { employee_id = employeeId });
	}
}

public static class Route
{
	public static IEndpointRouteBuilder MapGetEmployeeBusinessesEndpoint(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/api/businesses/employee", async (HttpContext context,
			BusinessManagementFacade facade
			) =>
		{
			await context.Response.WriteAsJsonAsync(new { Message = "Hello from the employee businesses endpoint" });
		});

		return endpoints;
	}
}