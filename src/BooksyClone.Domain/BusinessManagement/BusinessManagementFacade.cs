using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Contract.Shared;
using BooksyClone.Domain.Availability.Storage;
using BooksyClone.Domain.BusinessManagement.ConfiguringServiceVariantsOfferedByBusiness;
using BooksyClone.Domain.BusinessManagement.FetchingBusinessConfiguration;
using BooksyClone.Domain.Storage;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Domain.BusinessManagement;

public class BusinessManagementFacade
{
    private readonly ConfigureServiceVariantsOfferedByBusiness _configureServiceVariantsOfferedByBusiness;
    private readonly GetBusinessConfiguration _getBusinessConfiguration;

    internal BusinessManagementFacade(
        ConfigureServiceVariantsOfferedByBusiness configureServiceVariantsOfferedByBusiness,
        GetBusinessConfiguration getBusinessConfiguration)
    {
        _configureServiceVariantsOfferedByBusiness = configureServiceVariantsOfferedByBusiness;
        _getBusinessConfiguration = getBusinessConfiguration;
    }

    public async Task<Result> ConfigureServicesOfferedByBusiness(
        BusinessServiceConfigurationDto businessServiceConfigurationDto,
        CancellationToken ct)
    {
        return await _configureServiceVariantsOfferedByBusiness.HandleAsync(businessServiceConfigurationDto, ct);
    }

    public async Task<object?> GetBusinessConfigurationAsync(Guid businessUnitId, CancellationToken ct)
    {
        return await _getBusinessConfiguration.HandleAsync(businessUnitId, ct);
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
            new ConfigureServiceVariantsOfferedByBusiness(new DbConnectionFactory(_configuration.GetPostgresDatabaseConnectionString())),
            new GetBusinessConfiguration(new DbConnectionFactory(_configuration.GetPostgresDatabaseConnectionString()))
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
        endpoints.MapGetBusinessConfigurationEndpoint();
    }
}