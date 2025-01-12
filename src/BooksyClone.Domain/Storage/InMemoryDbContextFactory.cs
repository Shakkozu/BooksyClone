using Microsoft.EntityFrameworkCore;

namespace BooksyClone.Domain.Storage;

public class InMemoryDbContextFactory
{
    internal static PostgresDbContext Create()
    {
        var options = new DbContextOptionsBuilder<PostgresDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDb")
            .Options;

        var context = new PostgresDbContext(options);

        return context;
    }
}
