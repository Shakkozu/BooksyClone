using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksyClone.BusinessManagement.Migrations;

[Migration(2)]
public class AddServicesOfferedByBusinessTable : Migration
{
    public override void Down()
    {
        Delete.Table("services_offered_by_business_employee").InSchema("business_management");
    }

    public override void Up()
    {
        Create.Table("services_offered_by_business_employee")
            .InSchema("business_management")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("business_id").AsInt64().NotNullable()
            .WithColumn("employee_id").AsInt64().NotNullable()
            .WithColumn("offered_services_definition").AsCustom("jsonb").NotNullable()
            .WithColumn("version").AsInt64().NotNullable();

        Create.ForeignKey("fk_services_offered_by_business_employee_business_id")
            .FromTable("services_offered_by_business_employee").InSchema("business_management")
            .ForeignColumn("business_id")
            .ToTable("business_drafts").InSchema("business_management")
            .PrimaryColumn("id");
    }
}