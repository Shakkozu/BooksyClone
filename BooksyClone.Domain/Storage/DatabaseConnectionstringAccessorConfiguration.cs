using Microsoft.Extensions.Configuration;

namespace BooksyClone.Domain.Storage;
internal static class DatabaseConnectionstringAccessorConfiguration
{
	internal static string GetPostgresDatabaseConnectionString(this IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("Postgres");
		if (connectionString is null)
			throw new ArgumentNullException("connectionstring cannot be empty");

		return connectionString;
	}
}
