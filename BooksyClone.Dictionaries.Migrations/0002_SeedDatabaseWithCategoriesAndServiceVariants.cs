using FluentMigrator;

namespace BooksyClone.Dictionaries.Migrations;

[Migration(2)]
public class SeedDatabaseWithCategoriesAndServiceVariants : Migration
{
    public override void Up()
    {
        var categoryGuid = Guid.NewGuid();

        Execute.Sql($@"
            INSERT INTO dictionaries.categories (guid, name, created_at, updated_at) 
            VALUES ('{categoryGuid}', 'Fryzjer', NOW(), NOW());
        ");

        Execute.Sql($@"
            INSERT INTO dictionaries.service_variant (guid, category_id, name, created_at, updated_at) 
            VALUES 
            (uuid_generate_v4(), (SELECT id FROM dictionaries.categories WHERE guid = '{categoryGuid}'), 'Strzyżenie męskie', NOW(), NOW()),
            (uuid_generate_v4(), (SELECT id FROM dictionaries.categories WHERE guid = '{categoryGuid}'), 'Strzyżenie damskie', NOW(), NOW()),
            (uuid_generate_v4(), (SELECT id FROM dictionaries.categories WHERE guid = '{categoryGuid}'), 'Broda', NOW(), NOW()),
            (uuid_generate_v4(), (SELECT id FROM dictionaries.categories WHERE guid = '{categoryGuid}'), 'Fryzjer dla dzieci', NOW(), NOW()),
            (uuid_generate_v4(), (SELECT id FROM dictionaries.categories WHERE guid = '{categoryGuid}'), 'Koloryzacja i farbowanie włosów', NOW(), NOW());
        ");
    }

    public override void Down()
    {
        Delete.FromTable("service_variant").InSchema("dictionaries").Row(new { name = "Strzyżenie męskie" });
        Delete.FromTable("service_variant").InSchema("dictionaries").Row(new { name = "Strzyżenie damskie" });
        Delete.FromTable("service_variant").InSchema("dictionaries").Row(new { name = "Broda" });
        Delete.FromTable("service_variant").InSchema("dictionaries").Row(new { name = "Fryzjer dla dzieci" });
        Delete.FromTable("service_variant").InSchema("dictionaries").Row(new { name = "Koloryzacja i farbowanie włosów" });
        Delete.FromTable("categories").InSchema("dictionaries").Row(new { name = "Fryzjer" });
    }
}
