using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Routing;
using BooksyClone.Domain.Dictionaries.FetchingAvailableServicesVariants;

namespace BooksyClone.Domain.Dictionaries;

public static class DictionariesModule
{
	public static void InstallDictionariesModule(this IServiceCollection serviceProvider, IConfiguration configuration)
	{
		serviceProvider.AddTransient<DictionariesFacade>(sp =>
		{
			return new DictionariesFacadeBuilder(configuration).Build();
		});
	}

	public static IEndpointRouteBuilder MapDictionariesModuleEndpoints(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapFetchAvailableServiceVariantsEndpoint();

		return endpoints;
	}
}
