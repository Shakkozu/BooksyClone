using FluentMigrator;

namespace BooksyClone.Infrastructure.Migrations.Search;


[Migration(0001, "Add service type to time required table")]
public class _0001_AddServiceToTimeRequiredTable : Migration
{
	public override void Up()
	{
		Create.Table("test")
			.InSchema("search")
			.WithColumn("id").AsInt64().PrimaryKey().Identity()
			.WithColumn("guid").AsGuid().NotNullable().Unique().WithDefaultValue(SystemMethods.NewSequentialId);
	}

	public override void Down()
	{
		Delete.Table("test");
	}
}
