using FluentMigrator;
using Stingy.Migrations.Const;

namespace Stingy.Migrations.Migrations;

[Migration(20260108)]
public class CreateBudgetYearsTable : Migration
{
    private const string Schema = Schemas.Budgets;

    public override void Up()
    {
        Create.Table("BudgetYears").InSchema(Schema)
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("BudgetId").AsGuid().NotNullable()
            .WithColumn("Value").AsInt32().NotNullable()
            .WithColumn("Status").AsInt32().NotNullable();

        Create.ForeignKey("FK_BudgetYears_Budgets")
            .FromTable("BudgetYears").InSchema(Schema).ForeignColumn("BudgetId")
            .ToTable("Budgets").InSchema(Schema).PrimaryColumn("Id");

        Create.Index("IX_BudgetYears_BudgetId")
            .OnTable("BudgetYears").InSchema(Schema)
            .OnColumn("BudgetId");

        Create.UniqueConstraint("IX_BudgetYears_BudgetId_Value")
            .OnTable("BudgetYears").WithSchema(Schema)
            .Columns("BudgetId", "Value");
    }

    public override void Down()
    {
        Delete.Table("BudgetYears").InSchema(Schema);
    }
}
