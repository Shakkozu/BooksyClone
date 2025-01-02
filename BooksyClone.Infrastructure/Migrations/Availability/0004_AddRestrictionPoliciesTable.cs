using FluentMigrator;
using FluentMigrator.Model;

namespace BooksyClone.Infrastructure.Migrations.Availability;

[Migration(0004, "add policies table")]
public class AddRestrictionPoliciesTable : Migration
{
	public override void Up()
	{
		Create.Table("resource_locking_restriction_policy")
			.WithColumn("id").AsInt64().PrimaryKey().Identity()
			.WithColumn("resource_id").AsGuid().NotNullable().ForeignKey("fk_resource_locking_restriction_policy_resource", "resource", "correlation_id")
			.WithColumn("created_by").AsGuid().NotNullable()
			.WithColumn("timestamp").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
			.WithColumn("definition").AsCustom("jsonb").NotNullable()
			.WithColumn("from").AsDateTime().NotNullable()
			.WithColumn("to").AsDateTime().NotNullable();

		Execute.Sql(@"
            ALTER TABLE resource_locking_restriction_policy
            ADD CONSTRAINT resource_locking_restriction_policy_no_overlap
            EXCLUDE USING gist (
                resource_id WITH =,
                tsrange(""from"", ""to"") WITH &&
            );
        ");
	}
	public override void Down()
	{
		Execute.Sql("DROP TABLE IF EXISTS resource_locking_restriction_policy;");
	}
}