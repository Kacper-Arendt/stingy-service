using FluentMigrator;
using Stingy.Migrations.Const;

namespace Stingy.Migrations.Migrations;

[Migration(20250714)]
public class CreateTeamsTables : Migration
{
    private const string Schema = Schemas.Teams;

    public override void Up()
    {
        Execute.Sql($"""
                     IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = '{Schema}')
                                     EXEC('CREATE SCHEMA [{Schema}]');
                     """);

        // Teams table
        Create.Table("Teams").InSchema(Schema)
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("Description").AsString(500).Nullable()
            .WithColumn("CreatedAt").AsDateTime2().NotNullable()
            .WithColumn("CreatedBy").AsGuid().NotNullable();

        Create.Index("IX_Teams_CreatedBy").OnTable("Teams").InSchema(Schema).OnColumn("CreatedBy");
        Create.Index("IX_Teams_Name").OnTable("Teams").InSchema(Schema).OnColumn("Name");

        // TeamParticipants table
        Create.Table("TeamParticipants").InSchema(Schema)
            .WithColumn("UserId").AsGuid().NotNullable()
            .WithColumn("TeamId").AsGuid().NotNullable()
            .WithColumn("Email").AsString(256).NotNullable()
            .WithColumn("Role").AsInt32().NotNullable()
            .WithColumn("Status").AsInt32().NotNullable()
            .WithColumn("JoinedAt").AsDateTime2().NotNullable();

        Create.PrimaryKey("PK_TeamParticipants")
            .OnTable("TeamParticipants").WithSchema(Schema)
            .Columns("UserId", "TeamId");

        Create.Index("IX_TeamParticipants_TeamId").OnTable("TeamParticipants").InSchema(Schema).OnColumn("TeamId");
        Create.Index("IX_TeamParticipants_Email").OnTable("TeamParticipants").InSchema(Schema).OnColumn("Email");
        Create.Index("IX_TeamParticipants_Status").OnTable("TeamParticipants").InSchema(Schema).OnColumn("Status");

        Create.ForeignKey("FK_TeamParticipants_Teams")
            .FromTable("TeamParticipants").InSchema(Schema).ForeignColumn("TeamId")
            .ToTable("Teams").InSchema(Schema).PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
        Delete.Table("TeamParticipants").InSchema(Schema);
        Delete.Table("Teams").InSchema(Schema);
    }
} 