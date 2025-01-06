using BooksyClone.Infrastructure.Migrations.Availability;
using BooksyClone.Infrastructure.Migrations.Search;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Infrastructure.Migrations;
public static class MigrationsRunner
{	public static void RunMigrations(string connectionString)
	{
		// Configure migrations for Availability module
		var availabilityServices = new ServiceCollection()
			.AddFluentMigratorCore()
			.ConfigureRunner(rb => rb
				.AddPostgres()
				.WithGlobalConnectionString(connectionString)
				.ScanIn(typeof(AddResourcesTable).Assembly).For.Migrations()
				)
			.AddLogging(lb => lb.AddFluentMigratorConsole());

		// Configure migrations for Search module
		var searchServices = new ServiceCollection()
			.AddFluentMigratorCore()
			.ConfigureRunner(rb => rb
				.AddPostgres()
				.WithGlobalConnectionString(connectionString)
				.ScanIn(typeof(_0001_AddServiceToTimeRequiredTable).Assembly).For.Migrations()
				.WithVersionTable(new DefaultVersionTableMetaData("search")))
			.AddLogging(lb => lb.AddFluentMigratorConsole());

		// Build service providers for each module
		var availabilityServiceProvider = availabilityServices.BuildServiceProvider();
		var searchServiceProvider = searchServices.BuildServiceProvider();

		// Run migrations for each module
		using (var availabilityScope = availabilityServiceProvider.CreateScope())
		{
			var availabilityRunner = availabilityScope.ServiceProvider.GetRequiredService<IMigrationRunner>();
			availabilityRunner.MigrateUp();
		}

		using (var searchScope = searchServiceProvider.CreateScope())
		{
			var searchRunner = searchScope.ServiceProvider.GetRequiredService<IMigrationRunner>();
			searchRunner.MigrateUp();
		}
	}
}
