using FluentMigrator;
using Stingy.Migrations.Const;

namespace Stingy.Migrations.Migrations;

[Migration(20250829)]
public class CreateUserProfilesTables : Migration
{
    private const string Schema = Schemas.Users;

    public override void Up()
    {
        Execute.Sql($"""
                     IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = '{Schema}')
                                     EXEC('CREATE SCHEMA [{Schema}]');
                     """);
        
        Create.Table("UserProfiles").InSchema(Schema)
            .WithColumn("UserId").AsString(450).PrimaryKey().NotNullable()
            .WithColumn("DisplayName").AsString(50).NotNullable()
            .WithColumn("ProfileImageUrl").AsString(500).Nullable()
            .WithColumn("Bio").AsString(500).Nullable()
            .WithColumn("TimeZone").AsString(50).NotNullable().WithDefaultValue("Europe/Warsaw")
            .WithColumn("Visibility").AsInt32().NotNullable().WithDefaultValue(0) // 0=Public
            .WithColumn("CreatedAt").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("LastModifiedAt").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime);

        Create.Table("ProductTourPostStatus").InSchema(Schema)
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("UserId").AsGuid().NotNullable()
            .WithColumn("TourKey").AsString(100).NotNullable()
            .WithColumn("Status").AsInt32().NotNullable();

        Create.UniqueConstraint("UQ_ProductTourPostStatus_UserId_TourKey")
            .OnTable("ProductTourPostStatus").WithSchema(Schema)
            .Columns("UserId", "TourKey");
        
        // Indexes
        Create.Index("IX_UserProfiles_DisplayName").OnTable("UserProfiles").InSchema(Schema).OnColumn("DisplayName").Unique();
        Create.Index("IX_UserProfiles_Visibility").OnTable("UserProfiles").InSchema(Schema).OnColumn("Visibility");
        Create.Index("IX_UserProfiles_CreatedAt").OnTable("UserProfiles").InSchema(Schema).OnColumn("CreatedAt");
        
    }

    public override void Down()
    {
        Delete.Table("UserProfiles").InSchema(Schema);
    }
}
