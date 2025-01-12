using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Domain.Storage;

public static class EntityframeworkInstaller
{
    public static void InstallPostgresEntityFramework(this IServiceCollection serviceProvider, string connectionString)
    {
        serviceProvider.AddSingleton(_=> new PostgresDbContext(connectionString));
        serviceProvider.AddDbContext<PostgresDbContext>();
    }
}
