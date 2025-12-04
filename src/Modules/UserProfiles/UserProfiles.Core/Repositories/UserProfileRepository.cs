using Dapper;
using Microsoft.Data.SqlClient;
using UserProfiles.Core.Repositories.Models;
using UserProfiles.Domain.Entities;
using UserProfiles.Domain.Enums;
using UserProfiles.Domain.ValueObjects;
using Shared.Abstractions.Factories;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly IDbConnectionFactory _factory;

    public UserProfileRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<UserProfile?> GetByUserIdAsync(UserId userId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           SELECT UserId, DisplayName, ProfileImageUrl, Bio, TimeZone, Visibility, 
                                  CreatedAt, LastModifiedAt
                           FROM [UserProfiles].[UserProfiles]
                           WHERE UserId = @UserId
                           """;

        var row = await dbConnection.QuerySingleOrDefaultAsync<UserProfileDbRow>(sql, new { UserId = userId.Value });
        return row == null ? null : MapFromDbRow(row);
    }

    public async Task<UserProfile?> GetByDisplayNameAsync(DisplayName displayName)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           SELECT UserId, DisplayName, ProfileImageUrl, Bio, TimeZone, Visibility, 
                                  CreatedAt, LastModifiedAt
                           FROM [UserProfiles].[UserProfiles]
                           WHERE DisplayName = @DisplayName
                           """;

        var row = await dbConnection.QuerySingleOrDefaultAsync<UserProfileDbRow>(sql, new { DisplayName = displayName.Value });
        return row == null ? null : MapFromDbRow(row);
    }

    public async Task<bool> ExistsAsync(UserId userId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           SELECT 1
                           FROM [UserProfiles].[UserProfiles]
                           WHERE UserId = @UserId
                           """;

        var result = await dbConnection.ExecuteScalarAsync<int?>(sql, new { UserId = userId.Value });
        return result.HasValue;
    }

    public async Task<bool> IsDisplayNameTakenAsync(DisplayName displayName, UserId? excludeUserId = null)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        string sql;
        object parameters;

        if (excludeUserId != null)
        {
            sql = """
                  SELECT 1
                  FROM [UserProfiles].[UserProfiles]
                  WHERE DisplayName = @DisplayName AND UserId != @ExcludeUserId
                  """;
            parameters = new { DisplayName = displayName.Value, ExcludeUserId = excludeUserId.Value };
        }
        else
        {
            sql = """
                  SELECT 1
                  FROM [UserProfiles].[UserProfiles]
                  WHERE DisplayName = @DisplayName
                  """;
            parameters = new { DisplayName = displayName.Value };
        }

        var result = await dbConnection.ExecuteScalarAsync<int?>(sql, parameters);
        return result.HasValue;
    }

    public async Task SaveAsync(UserProfile userProfile)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           INSERT INTO [UserProfiles].[UserProfiles] 
                           (UserId, DisplayName, ProfileImageUrl, Bio, TimeZone, Visibility, 
                            CreatedAt, LastModifiedAt)
                           VALUES 
                           (@UserId, @DisplayName, @ProfileImageUrl, @Bio, @TimeZone, @Visibility, 
                            @CreatedAt, @LastModifiedAt)
                           """;

        await dbConnection.ExecuteAsync(sql, new
        {
            UserId = userProfile.UserId.Value,
            DisplayName = userProfile.DisplayName.Value,
            ProfileImageUrl = userProfile.ProfileImageUrl?.Value,
            Bio = userProfile.Bio?.Value,
            TimeZone = userProfile.TimeZone.Value,
            Visibility = (int)userProfile.Visibility,
            CreatedAt = userProfile.CreatedAt.Value,
            LastModifiedAt = userProfile.LastModifiedAt.Value
        });
    }

    public async Task UpdateAsync(UserProfile userProfile)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());

        const string sql = """
                           UPDATE [UserProfiles].[UserProfiles]
                           SET DisplayName = @DisplayName,
                               ProfileImageUrl = @ProfileImageUrl,
                               Bio = @Bio,
                               TimeZone = @TimeZone,
                               Visibility = @Visibility,
                               LastModifiedAt = @LastModifiedAt
                           WHERE UserId = @UserId
                           """;

        await dbConnection.ExecuteAsync(sql, new
        {
            UserId = userProfile.UserId.Value,
            DisplayName = userProfile.DisplayName.Value,
            ProfileImageUrl = userProfile.ProfileImageUrl?.Value,
            Bio = userProfile.Bio?.Value,
            TimeZone = userProfile.TimeZone.Value,
            Visibility = (int)userProfile.Visibility,
            LastModifiedAt = userProfile.LastModifiedAt.Value
        });
    }

    private static UserProfile MapFromDbRow(UserProfileDbRow row)
    {
        return new UserProfile.UserProfileBuilder()
            .WithUserId(new UserId(Guid.Parse(row.UserId)))
            .WithDisplayName(new DisplayName(row.DisplayName))
            .WithProfileImageUrl(string.IsNullOrWhiteSpace(row.ProfileImageUrl) ? null : new ProfileImageUrl(row.ProfileImageUrl))
            .WithBio(string.IsNullOrWhiteSpace(row.Bio) ? null : new UserBio(row.Bio))
            .WithTimeZone(new DateTimeZone(row.TimeZone))
            .WithVisibility((ProfileVisibilityLevel)row.Visibility)
            .WithCreatedAt(new CreatedAtDate(row.CreatedAt))
            .WithLastModifiedAt(new LastModifiedDate(row.LastModifiedAt))
            .Build();
    }
}
