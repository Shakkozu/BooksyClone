using FluentMigrator;

namespace BooksyClone.BusinessManagement.Migrations;

[Migration(1)]
public class CreateBusinessDraftsTable : Migration
{
    public override void Up()
    {
        Create.Table("business_drafts")
			.InSchema("business_management")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("business_details").AsCustom("JSONB").NotNullable()
            .WithColumn("user_details").AsCustom("JSONB").NotNullable()
            .WithColumn("business_proof_document_data").AsCustom("BYTEA").NotNullable()
            .WithColumn("business_proof_document_file_name").AsString(255).NotNullable()
            .WithColumn("business_proof_document_content_type").AsString(100).NotNullable()
            .WithColumn("user_identity_document_data").AsCustom("BYTEA").NotNullable()
            .WithColumn("user_identity_document_file_name").AsString(255).NotNullable()
            .WithColumn("user_identity_document_content_type").AsString(100).NotNullable()
            .WithColumn("status").AsString(50).NotNullable()
            .WithColumn("legal_consent_content").AsString().NotNullable()
            .WithColumn("legal_consent").AsBoolean().NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable()
            .WithColumn("updated_at").AsDateTime().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("business_drafts").InSchema("business_management");
    }
}
