using FluentMigrator;

namespace BooksyClone.Schedules.Migrations;

[Migration(1)]
public class CreateSchedulesTables : Migration
{
    public override void Up()
    {
        Create.Table("monthly_schedule_definitions")
			.InSchema(SchedulesMigrationsConfiguration.SchemaName)
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("definition").AsCustom("JSONB").NotNullable()
            .WithColumn("year").AsInt32().NotNullable()
            .WithColumn("month").AsInt32().NotNullable()
            .WithColumn("modified_at").AsDateTime().NotNullable()
            .WithColumn("employee_id").AsGuid().NotNullable()
            .WithColumn("business_unit_id").AsGuid().NotNullable()
            .WithColumn("published_by").AsGuid().Nullable()
            .WithColumn("status").AsString(50).NotNullable();

        Create.UniqueConstraint("AK_monthly_schedule_definitions_year_month_employee_id_business_unit_id")
            .OnTable("monthly_schedule_definitions")
            .Columns("year", "month", "employee_id", "business_unit_id");

        Create.Table("schedule_business_units")
			.InSchema(SchedulesMigrationsConfiguration.SchemaName)
			.WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("business_unit_id").AsGuid().NotNullable()
            .WithColumn("employees_ids").AsCustom("JSONB").NotNullable();
    }

    public override void Down()
    {
        Delete.Table("monthly_schedule_definitions").InSchema(SchedulesMigrationsConfiguration.SchemaName);
        Delete.Table("schedule_business_units").InSchema(SchedulesMigrationsConfiguration.SchemaName);
    }
}
