using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Contract.Shared;
using BooksyClone.Domain.BusinessManagement.ConfiguringServiceVariantsOfferedByBusiness;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Domain.BusinessManagement;

public class BusinessManagementFacade
{
    private readonly ConfigureServiceVariantsOfferedByBusiness _configureServiceVariantsOfferedByBusiness;

    internal BusinessManagementFacade(ConfigureServiceVariantsOfferedByBusiness configureServiceVariantsOfferedByBusiness)
    {
        _configureServiceVariantsOfferedByBusiness = configureServiceVariantsOfferedByBusiness;
    }

    public async Task<Result> ConfigureServicesOfferedByBusiness(BusinessConfigurationDto businessConfigurationDto, CancellationToken ct)
    {
        return await _configureServiceVariantsOfferedByBusiness.HandleAsync(businessConfigurationDto, ct);
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