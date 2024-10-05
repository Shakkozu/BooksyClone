using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Domain.Storage;

public static class SqliteEntityframeworkInstaller
{

    public static void InstallSqliteEntityFramework(this IServiceCollection serviceProvider)
    {
        serviceProvider.AddSingleton(_ => SqliteDbContext.CreateInMemoryDatabase());
        serviceProvider.AddDbContext<SqliteDbContext>();
        serviceProvider.AddTransient<DbContext>(ctx => ctx.GetRequiredService<SqliteDbContext>());
    }
}