using BooksyClone.Domain.BusinessOnboarding.Model;
using BooksyClone.Domain.BusinessOnboarding.Storage;
using BooksyClone.Domain.Schedules.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BooksyClone.Domain.Storage;

internal class PostgresDbContext : DbContext
{
    private readonly string _connectionString;

    public PostgresDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresBusinessDraftsSchema.MapUsing(modelBuilder);
        PostgresSchedulesSchema.MapUsing(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder
          .LogTo(Console.WriteLine, LogLevel.Information)
          .EnableSensitiveDataLogging()
          .EnableDetailedErrors();
        base.OnConfiguring(optionsBuilder);
    }

    internal DbSet<BusinessDraft> BusinessDrafts { get; set; }
    internal DbSet<MonthlyScheduleDefinition> MonthlySchedules { get; set; }
    internal DbSet<ScheduleBusinessUnit> ScheduleBusiness { get; set; }
}
