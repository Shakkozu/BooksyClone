using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BooksyClone.Domain.Dictionaries.FetchingAvailableServicesVariants;

internal static class Route
{
	internal static IEndpointRouteBuilder MapFetchAvailableServiceVariantsEndpoint(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/api/v1/dictionaries/service-variants", async (
			HttpContext context,
			CancellationToken ct,
			DictionariesFacade dictionariesFacade) =>
		{
			await context.Response.WriteAsJsonAsync(await dictionariesFacade.FindAvailableServiceVariants(ct));
		});
		return endpoints;
	}
}
