using BooksyClone.Contract.Schedules;
using BooksyClone.Domain.Storage;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BooksyClone.Domain.Schedules.Storage;

internal static class SqliteSchedulesSchema
{
    internal static void MapUsing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MonthlyScheduleDefinition>(builder =>
        {
            builder.MapBaseEntityProperties();

            // Map complex types as JSON
            builder.Property(x => x.Definition)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<IEnumerable<MonthlyScheduleDefinitionDto>>(v)!)
                .HasColumnType("TEXT");

            builder.Property(x => x.Year).IsRequired();
            builder.Property(x => x.Month).IsRequired();
            builder.Property(x => x.ModifiedAt).IsRequired();
            builder.Property(x => x.EmployeeId).IsRequired();
            builder.Property(x => x.BusinessUnitId).IsRequired();
            builder.Property(x => x.PublishedBy);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);

            builder.HasAlternateKey(monthlySchedule => new
            {
                monthlySchedule.Year,
                monthlySchedule.Month,
                monthlySchedule.EmployeeId,
                monthlySchedule.BusinessUnitId
            });
        });

        modelBuilder.Entity<ScheduleBusinessUnit>(builder =>
        {
            builder.MapBaseEntityProperties();
            builder.Property(x => x.BusinessUnitId).IsRequired();
            builder.Property(x => x.EmployeesIds).HasConversion
            (
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<IEnumerable<Guid>>(v)!);
        });
    }

}

internal static class PostgresSchedulesSchema
{
    internal static void MapUsing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MonthlyScheduleDefinition>(builder =>
        {
            builder.MapBaseEntityProperties();

            // Map complex types as JSONB
            builder.Property(x => x.Definition)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<IEnumerable<MonthlyScheduleDefinitionDto>>(v)!)
                .HasColumnName("definition")
                .HasColumnType("JSONB");

            builder.Property(x => x.Year).IsRequired().HasColumnName("year");
            builder.Property(x => x.Month).IsRequired().HasColumnName("month");
            builder.Property(x => x.ModifiedAt).IsRequired().HasColumnName("modified_at");
            builder.Property(x => x.EmployeeId).IsRequired().HasColumnName("employee_id");
            builder.Property(x => x.BusinessUnitId).IsRequired().HasColumnName("business_unit_id");
            builder.Property(x => x.PublishedBy).HasColumnName("published_by");
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasColumnName("status");

            builder.HasAlternateKey(monthlySchedule => new
            {
                monthlySchedule.Year,
                monthlySchedule.Month,
                monthlySchedule.EmployeeId,
                monthlySchedule.BusinessUnitId
            }).HasName("AK_monthly_schedule_definitions_year_month_employee_id_business_unit_id");
        });

        modelBuilder.Entity<ScheduleBusinessUnit>(builder =>
        {
            builder.MapBaseEntityProperties();
            builder.Property(x => x.BusinessUnitId).IsRequired().HasColumnName("business_unit_id");
            builder.Property(x => x.EmployeesIds).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<IEnumerable<Guid>>(v)!)
                .HasColumnName("employees_ids")
                .HasColumnType("JSONB");
        });
    }
}

internal class ScheduleBusinessUnit : BaseEntity
{
    public Guid BusinessUnitId { get; set; }

    public IEnumerable<Guid> EmployeesIds { get; set; }
}
