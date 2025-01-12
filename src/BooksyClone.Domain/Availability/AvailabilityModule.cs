using BooksyClone.Domain.Availability.LockingTimeslotOnResource;
using BooksyClone.Infrastructure.TimeManagement;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace BooksyClone.Domain.Availability;

public static class AvailabilityModule
{
    internal const string AvailabilityConnectionStringKey = $"Availability:Postgres:ConnectionString";
    public static IServiceCollection InstallAvailabilityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITimeService>(new TimeService());
        services.AddTransient<AvailabilityBuilder>();
        services.AddTransient<AvailabilityFacade>(sp =>
        {
            var builder = sp.GetRequiredService<AvailabilityBuilder>();
            return builder.GetFacade();
        });
        services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(configuration.GetValue<string>(AvailabilityConnectionStringKey)));
        return services;
    }
    

    public static IEndpointRouteBuilder InstallAvailabilityModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // todo add endpoints
        return endpoints;

    }
}
