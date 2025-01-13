using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Contract.Shared;
using BooksyClone.Domain.BusinessManagement.ConfiguringServiceVariantsOfferedByBusiness;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Domain.BusinessManagement;

public class BusinessManagementFacade
{
    public Task<Result> ConfigureServicesOfferedByBusiness(BusinessConfigurationDto businessConfigurationDto)
    {
        throw new NotImplementedException();
    }
}

public static class BusinessManagementModule
{
    public static void InstallBusinessManagementModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<BusinessManagementFacade>();
    }

    public static void MapBusinessManagementEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapConfigureServiceVariantsOfferedByBusinessEndpoint();
    }
}