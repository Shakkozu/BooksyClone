using BooksyClone.Infrastructure.Migrations.Availability;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Infrastructure.Migrations;
public static class MigrationsRunner
{
    public static void ConfigureFluentMigrator(this IServiceCollection serviceDescriptors, string connectionString)
    {
        serviceDescriptors.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
            .AddPostgres()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(typeof(AddResourcesTable).Assembly).For.Migrations())
        .AddLogging(lb => lb.AddFluentMigratorConsole());
    }

    public static void RunFluentMigrator(this IServiceScope scope)
    {
        scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
    }
}
