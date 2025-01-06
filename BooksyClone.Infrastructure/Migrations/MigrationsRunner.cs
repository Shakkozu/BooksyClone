using BooksyClone.Auth.Migrations;
using BooksyClone.Infrastructure.Migrations.Availability;
using BooksyClone.Infrastructure.Migrations.Search;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Infrastructure.Migrations;
public static class MigrationsRunner
{
	public static void RunMigrations(string connectionString)
	{
		RunModuleMigrations<AddResourcesTable>(connectionString);
		RunModuleMigrations<_0001_AddServiceToTimeRequiredTable>(connectionString, "search");
		RunModuleMigrations<Migration0001_CreateUserContext>(connectionString, "auth");
	}

	private static void RunModuleMigrations<TMigration>(string connectionString, string schemaName = "public")
	{
		var versionTableMetadata = string.IsNullOrEmpty(schemaName) ? new DefaultVersionTableMetaData() : new DefaultVersionTableMetaData(schemaName);
		var services = new ServiceCollection()
			.AddFluentMigratorCore()
			.ConfigureRunner(rb => rb
				.AddPostgres()
				.WithGlobalConnectionString(connectionString)
				.ScanIn(typeof(TMigration).Assembly).For.Migrations()
				.WithVersionTable(versionTableMetadata))
			.AddLogging(lb => lb.AddFluentMigratorConsole())
			.BuildServiceProvider();

		using (var scope = services.CreateScope())
		{
			var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
			runner.MigrateUp();
		}
	}
}
