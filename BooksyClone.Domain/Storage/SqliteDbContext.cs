using BooksyClone.Domain.BusinessOnboarding;
using BooksyClone.Domain.BusinessOnboarding.Model;
using BooksyClone.Domain.BusinessOnboarding.Storage;
using BooksyClone.Domain.Schedules.Storage;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace BooksyClone.Domain.Storage;

public class SqliteDbContext : DbContext
{
    private readonly DbConnection _connection;

    public static DbConnection CreateInMemoryDatabase()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        return connection;
    }
    public SqliteDbContext(DbConnection connection)
    {
        _connection = connection;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SqliteBusinessDraftsSchema.MapUsing(modelBuilder);
        SqliteSchedulesSchema.MapUsing(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies().UseSqlite(_connection);
        optionsBuilder
          .LogTo(Console.WriteLine, LogLevel.Information)
          .EnableSensitiveDataLogging()
          .EnableDetailedErrors();
        base.OnConfiguring(optionsBuilder);
       
    }

    public async Task MigrateAsync()
    {
        await Database.EnsureCreatedAsync();
    }


    internal DbSet<BusinessDraft> BusinessDrafts { get; set; }
    internal DbSet<MonthlyScheduleDefinition> MonthlySchedules{ get; set; }
    internal DbSet<ScheduleBusinessUnit> ScheduleBusiness { get; set; }

}
