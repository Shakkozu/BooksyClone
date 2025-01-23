using BooksyClone.Domain.BusinessManagement.ConfiguringServiceVariantsOfferedByBusiness;
using BooksyClone.Domain.BusinessManagement.FetchingBusinessConfiguration;
using BooksyClone.Domain.BusinessManagement.FetchingEmployeeBusinesses;
using BooksyClone.Infrastructure.EmailSending;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Domain.BusinessManagement;

public static class BusinessManagementModule
{
	public static void InstallBusinessManagementModule(this IServiceCollection services, IConfiguration config)
	{
		services.AddTransient<BusinessManagementFacade>(sp =>
		{
			var builder = new BusinessManagementBuilder(config);
			return builder.Build(services);
		});

		services.AddEmailSender(config);
	}

	public static void MapBusinessManagementEndpoints(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapConfigureServiceVariantsOfferedByBusinessEndpoint();
		endpoints.MapGetBusinessConfigurationEndpoint();
		endpoints.MapGetEmployeeBusinessesEndpoint();
	}
}