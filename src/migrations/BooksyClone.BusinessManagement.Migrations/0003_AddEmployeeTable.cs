using FluentMigrator;

namespace BooksyClone.BusinessManagement.Migrations;

[Migration(3)]
public class AddEmployeeTable : Migration
{
    public override void Up()
    {
        Create.Table("employees")
            .InSchema("business_management")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("guid").AsGuid().NotNullable()
            .WithColumn("user_id").AsGuid().Nullable()
            .WithColumn("business_id").AsGuid().NotNullable()
            .WithColumn("registered_at").AsDateTime().NotNullable()
            .WithColumn("active_from").AsDateTime().NotNullable()
            .WithColumn("birth_date").AsDateTime().NotNullable()
            .WithColumn("active_to").AsDateTime().Nullable()
            .WithColumn("first_name").AsString(255).NotNullable()
            .WithColumn("last_name").AsString(255).NotNullable()
            .WithColumn("email").AsString(1000).NotNullable()
            .WithColumn("phone_number").AsString(255).Nullable();

        Create.Table("employees_invitation")
            .InSchema("business_management")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("guid").AsGuid().NotNullable()
            .WithColumn("business_id").AsGuid().NotNullable()
            .WithColumn("employee_id").AsGuid().NotNullable()
            .WithColumn("email").AsString(1000).NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable()
            .WithColumn("valid_to").AsDateTime().NotNullable()
            .WithColumn("used_at").AsDateTime().Nullable()
            .WithColumn("invitation_token").AsString(255).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("employees").InSchema("business_management");
        Delete.Table("employees_invitation").InSchema("business_management");
    }
}