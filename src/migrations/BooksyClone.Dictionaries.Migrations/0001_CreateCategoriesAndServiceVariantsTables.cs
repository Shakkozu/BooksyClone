using FluentMigrator;

namespace BooksyClone.Dictionaries.Migrations;

[Migration(1)]
public class CreateCategoriesAndServiceVariantsTables : Migration
{
    public override void Up()
    {
        Create.Table("categories")
            .InSchema(DictionariesMigrationsConfiguration.SchemaName)
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("guid").AsGuid().NotNullable()
            .WithColumn("name").AsString().NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.Table("service_variant")
            .InSchema(DictionariesMigrationsConfiguration.SchemaName)
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("guid").AsGuid().NotNullable()
            .WithColumn("category_id").AsInt32().NotNullable()
            .WithColumn("name").AsString().NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

		Create.ForeignKey("fk_service_variant_category_id")
			.FromTable("service_variant").InSchema(DictionariesMigrationsConfiguration.SchemaName)
			.ForeignColumn("category_id")
			.ToTable("categories").InSchema(DictionariesMigrationsConfiguration.SchemaName)
			.PrimaryColumn("id");
    }

    public override void Down()
    {
        Delete.Table("service_variant").InSchema(DictionariesMigrationsConfiguration.SchemaName);
        Delete.Table("categories").InSchema(DictionariesMigrationsConfiguration.SchemaName);
    }
}

public static class DictionariesMigrationsConfiguration
{
	public const string SchemaName = "dictionaries";
}