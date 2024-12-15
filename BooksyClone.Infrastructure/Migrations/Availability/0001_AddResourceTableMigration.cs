using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksyClone.Infrastructure.Migrations.Availability;


[Migration(0001, "add resources table")]
public class AddResourcesTable : Migration
{
    public override void Up()
    {
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS \"pgcrypto\";");
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");

        // Utwórz tabelę resource
        Create.Table("resource")
            .WithColumn("id").AsInt64().PrimaryKey()
            .WithColumn("guid").AsGuid().NotNullable().Unique().WithDefaultValue(SystemMethods.NewSequentialId)
            .WithColumn("correlation_id").AsGuid().NotNullable().Unique()
            .WithColumn("owner_id").AsGuid().NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        // Włącz rozszerzenie btree_gist (jeśli jeszcze nie istnieje)
    }

    public override void Down()
    {
        // Usuń tabelę resource
        Delete.Table("resource");
    }
}
