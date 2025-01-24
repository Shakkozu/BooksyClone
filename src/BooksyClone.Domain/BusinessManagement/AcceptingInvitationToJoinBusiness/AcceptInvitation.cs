using BooksyClone.Domain.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BooksyClone.Domain.BusinessManagement.AcceptingInvitationToJoinBusiness;

internal record AcceptInvitationRequest(string Token);
internal static class Route
{
	public static IEndpointRouteBuilder MapAcceptInvitationEndpoint(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapPost("/api/employee/companies/accept-invitation", async (
			HttpContext context,
			[FromBody] AcceptInvitationRequest payload,
			[FromServices] BusinessManagementFacade facade,
			[FromServices] AuthFacade authFacade) =>
		{
			var userId = authFacade.GetLoggedUserId();
			await facade.AcceptInvitation(userId, payload.Token);

			return Results.NoContent();
		});

		return endpoints;
	}
}
