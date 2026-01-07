using FluentMigrator;
using Stingy.Migrations.Const;

namespace Stingy.Migrations.Migrations;

[Migration(20260107)]
public class AddIsArchivedToBudgets : Migration
{
    private const string Schema = Schemas.Budgets;

    public override void Up()
    {
        Alter.Table("Budgets").InSchema(Schema)
            .AddColumn("IsArchived").AsBoolean().NotNullable().WithDefaultValue(false);
    }

    public override void Down()
    {
        Delete.Column("IsArchived").FromTable("Budgets").InSchema(Schema);
    }
}
