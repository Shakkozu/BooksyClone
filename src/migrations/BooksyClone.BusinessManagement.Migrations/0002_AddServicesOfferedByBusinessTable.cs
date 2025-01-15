using FluentMigrator;

namespace BooksyClone.BusinessManagement.Migrations;

[Migration(2)]
public class CreateEmployeeServicesTable : Migration
{
    public override void Up()
    {
        Create.Table("employee_services")
            .InSchema("business_management")
            .WithColumn("guid").AsGuid().PrimaryKey()
            .WithColumn("employee_id").AsGuid().NotNullable()
            .WithColumn("business_id").AsGuid().NotNullable()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("markdown_description").AsString().Nullable()
            .WithColumn("generic_service_variants_ids").AsCustom("jsonb").NotNullable()
            .WithColumn("duration").AsTime().NotNullable()
            .WithColumn("price").AsCustom("jsonb").NotNullable()
            .WithColumn("order").AsInt32().NotNullable()
            .WithColumn("category_id").AsInt64().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("employee_services").InSchema("business_management");
    }
}