using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Contract.Shared;
using BooksyClone.Domain.Availability.Storage;
using BooksyClone.Domain.BusinessManagement.ConfiguringServiceVariantsOfferedByBusiness;
using BooksyClone.Domain.Storage;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Domain.BusinessManagement;

public class BusinessManagementFacade
{
    private readonly ConfigureServiceVariantsOfferedByBusiness _configureServiceVariantsOfferedByBusiness;

    internal BusinessManagementFacade(
        ConfigureServiceVariantsOfferedByBusiness configureServiceVariantsOfferedByBusiness)
    {
        _configureServiceVariantsOfferedByBusiness = configureServiceVariantsOfferedByBusiness;
    }

    public async Task<Result> ConfigureServicesOfferedByBusiness(BusinessConfigurationDto businessConfigurationDto,
        CancellationToken ct)
    {
        return await _configureServiceVariantsOfferedByBusiness.HandleAsync(businessConfigurationDto, ct);
    }
}


internal class BusinessManagementBuilder
{
    private readonly IConfiguration _configuration;

    public BusinessManagementBuilder(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public BusinessManagementFacade Build()
    {
        return new BusinessManagementFacade(
            new ConfigureServiceVariantsOfferedByBusiness(
                new DbConnectionFactory(_configuration.GetPostgresDatabaseConnectionString())
            )
        );
    }
    
}
public static class BusinessManagementModule
{
    public static void InstallBusinessManagementModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<BusinessManagementFacade>(sp =>
        {
            var builder = new BusinessManagementBuilder(config);
            return builder.Build();
        });
    }

    public static void MapBusinessManagementEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapConfigureServiceVariantsOfferedByBusinessEndpoint();
    }
}