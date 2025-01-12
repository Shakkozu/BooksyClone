using FluentMigrator;

namespace BooksyClone.Auth.Migrations;

[Migration(1)]
public class Migration0001_CreateUserContext : Migration
{
	public override void Up()
	{
		Create.Table("users")
			.InSchema(AuthConstants.Schema)
			.WithColumn("id").AsString().NotNullable()
			.WithColumn("username").AsString().NotNullable()
			.WithColumn("normalized_username").AsString().NotNullable()
			.WithColumn("email").AsString().NotNullable()
			.WithColumn("normalized_email").AsString().NotNullable()
			.WithColumn("email_confirmed").AsBoolean().NotNullable()
			.WithColumn("password_hash").AsString().NotNullable()
			.WithColumn("security_stamp").AsString().NotNullable()
			.WithColumn("concurrency_stamp").AsString().NotNullable()
			.WithColumn("phone_number").AsString().Nullable()
			.WithColumn("phone_number_confirmed").AsBoolean().NotNullable()
			.WithColumn("two_factor_enabled").AsBoolean().NotNullable()
			.WithColumn("lockout_end").AsDateTime().Nullable()
			.WithColumn("lockout_enabled").AsBoolean().Nullable()
			.WithColumn("access_failed_count").AsInt32().NotNullable();

		Create.Table("roles")
			.InSchema(AuthConstants.Schema)
			.WithColumn("id").AsString()
			.WithColumn("name").AsString().NotNullable()
			.WithColumn("concurrency_stamp").AsString().Nullable()
			.WithColumn("normalized_name").AsString().NotNullable();

		Create.Table("user_roles")
			.InSchema(AuthConstants.Schema)
			.WithColumn("user_id").AsString().NotNullable()
			.WithColumn("role_id").AsString().NotNullable();

		Create.Table("user_claims")
			.InSchema(AuthConstants.Schema)
			.WithColumn("id").AsString()
			.WithColumn("user_id").AsString().NotNullable()
			.WithColumn("claim_type").AsString().NotNullable()
			.WithColumn("claim_value").AsString().NotNullable();
	}

	public override void Down()
	{
		Delete.Table("users").InSchema(AuthConstants.Schema);
		Delete.Table("roles").InSchema(AuthConstants.Schema);
		Delete.Table("user_roles").InSchema(AuthConstants.Schema);
		Delete.Table("user_claims").InSchema(AuthConstants.Schema);
	}
}