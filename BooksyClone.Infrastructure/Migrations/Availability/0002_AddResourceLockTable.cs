using FluentMigrator;

namespace BooksyClone.Infrastructure.Migrations.Availability;

[Migration(0002, "add resource lock table")]
public class AddResourceLockTable : Migration
{
    public override void Up()
    {
        Create.Table("resource_lock")
            .WithColumn("id").AsInt64().PrimaryKey()
            .WithColumn("guid").AsGuid().NotNullable().Unique().WithDefaultValue(SystemMethods.NewSequentialId)
            .WithColumn("resource_id").AsGuid().NotNullable()
            .ForeignKey("fk_resource_lock_resource", "resource", "guid")
            .WithColumn("created_by").AsGuid().NotNullable()
            .WithColumn("timestamp").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("from").AsDateTime().NotNullable()
            .WithColumn("to").AsDateTime().NotNullable();
    }

    public override void Down()
    {
        // Usuń tabelę resource_lock
        Delete.Table("resource_lock");
    }
}
