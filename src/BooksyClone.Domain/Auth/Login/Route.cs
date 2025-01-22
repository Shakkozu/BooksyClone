using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BooksyClone.Domain.Auth.Login;

internal static class Route
{
	internal static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder routeBuilder)
	{
		routeBuilder.MapPost("/api/accounts/login", async (HttpContext context,
			[FromServices] AuthFacade authFacade) =>
		{
			var command = await context.Request.ReadFromJsonAsync<LoginUserDto>();
			var result = await authFacade.LoginUserAsync(command, context.RequestAborted);
			if (result.Success)
			{
				await context.Response.WriteAsJsonAsync(result);
			}
			else
			{
				context.Response.StatusCode = 400;
				await context.Response.WriteAsJsonAsync(result);
			}
		});
		return routeBuilder;
	}
}