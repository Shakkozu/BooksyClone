﻿using BooksyClone.Domain.Auth.RegisterUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BooksyClone.Domain.Auth.RestrictedResource;

internal static class Route
{
	internal static IEndpointRouteBuilder MapGetRestrictedResourceEndpoint(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/api/accounts/restricted-resource",
			(HttpContext context,
			CancellationToken ct,
			AuthFacade authFacade) =>
			{
				var userId = authFacade.GetLoggedUserId();
				return Results.Ok(new { userId });
			}).RequireAuthorization();

		return endpoints;
	}
}