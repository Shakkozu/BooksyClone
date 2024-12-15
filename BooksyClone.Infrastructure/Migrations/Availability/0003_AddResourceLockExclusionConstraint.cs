using FluentMigrator;

namespace BooksyClone.Infrastructure.Migrations.Availability;

[Migration(0003, "add resource lock exclusiton constraint")]
public class AddResourceLockExclusionConstraint : Migration
{
    public override void Up()
    {
        // Ensure the `btree_gist` extension is enabled.
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");

        // Add the exclusion constraint to enforce no overlapping time ranges for the same resource.
        Execute.Sql(@"
            ALTER TABLE resource_lock
            ADD CONSTRAINT resource_lock_no_overlap
            EXCLUDE USING gist (
                resource_id WITH =,
                tsrange(""from"", ""to"") WITH &&
            );
        ");
    }

    public override void Down()
    {
        // Drop the exclusion constraint during rollback.
        Execute.Sql("ALTER TABLE resource_lock DROP CONSTRAINT IF EXISTS resource_lock_no_overlap;");
    }
}