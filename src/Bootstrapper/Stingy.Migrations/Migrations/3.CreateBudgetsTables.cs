using FluentMigrator;
using Stingy.Migrations.Const;

namespace Stingy.Migrations.Migrations;

[Migration(20260106)]
public class CreateBudgetsTables : Migration
{
    private const string Schema = Schemas.Budgets;

    public override void Up()
    {
        Execute.Sql($"""
                     IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = '{Schema}')
                                     EXEC('CREATE SCHEMA [{Schema}]');
                     """);

        // Budgets table
        Create.Table("Budgets").InSchema(Schema)
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Name").AsString(150).NotNullable()
            .WithColumn("Description").AsString(500).Nullable()
            .WithColumn("CreatedAt").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime);

        Create.Table("BudgetMembers").InSchema(Schema)
            .WithColumn("BudgetId").AsGuid().NotNullable()
            .WithColumn("UserId").AsGuid().NotNullable()
            .WithColumn("Email").AsString(256).NotNullable()
            .WithColumn("Role").AsInt32().NotNullable()
            .WithColumn("Status").AsInt32().NotNullable()
            .WithColumn("JoinedAt").AsDateTime2().NotNullable();

        Create.PrimaryKey("PK_BudgetMembers")
            .OnTable("BudgetMembers").WithSchema(Schema)
            .Columns("BudgetId", "UserId");

        Create.Index("IX_BudgetMembers_BudgetId").OnTable("BudgetMembers").InSchema(Schema).OnColumn("BudgetId");
        Create.Index("IX_BudgetMembers_UserId").OnTable("BudgetMembers").InSchema(Schema).OnColumn("UserId");

    }

    public override void Down()
    {
        Delete.Table("Budgets").InSchema(Schema);
        Delete.Table("BudgetMembers").InSchema(Schema);
    }
}
