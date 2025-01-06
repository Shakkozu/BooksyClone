using FluentMigrator;

namespace BooksyClone.Infrastructure.Migrations.Availability;


[Migration(0001, "add resources table")]
public class AddResourcesTable : Migration
{
    public override void Up()
    {
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS \"pgcrypto\";");
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");

        Create.Table("resource")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("guid").AsGuid().NotNullable().Unique().WithDefaultValue(SystemMethods.NewSequentialId)
            .WithColumn("correlation_id").AsGuid().NotNullable().Unique()
            .WithColumn("owner_id").AsGuid().NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);
    }

    public override void Down()
    {
        Delete.Table("resource");
    }
}
