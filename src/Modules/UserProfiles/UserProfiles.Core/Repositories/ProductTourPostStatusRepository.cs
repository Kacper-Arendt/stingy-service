using Dapper;
using Microsoft.Data.SqlClient;
using UserProfiles.Core.Repositories.Models;
using UserProfiles.Domain.Entities;
using UserProfiles.Domain.Enums;
using UserProfiles.Domain.ValueObjects;
using Shared.Abstractions.Factories;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Repositories;

public class ProductTourPostStatusRepository : IProductTourPostStatusRepository
{
    private readonly IDbConnectionStringFactory _stringFactory;

    public ProductTourPostStatusRepository(IDbConnectionStringFactory stringFactory)
    {
        _stringFactory = stringFactory;
    }

    public async Task<ProductTourPostStatus?> GetByUserAndTourKeyAsync(UserId userId, TourKey tourKey)
    {
        await using var dbConnection = new SqlConnection(_stringFactory.GetDefault());

        const string sql = """
                           SELECT Id, UserId, TourKey, Status
                           FROM [dbo].[ProductTourPostStatus]
                           WHERE UserId = @UserId AND TourKey = @TourKey
                           """;

        var row = await dbConnection.QuerySingleOrDefaultAsync<ProductTourPostStatusDbRow>(sql, 
            new { UserId = userId.Value, TourKey = tourKey.Value });
        
        return row == null ? null : MapFromDbRow(row);
    }

    public async Task<List<ProductTourPostStatus>> GetAllByUserIdAsync(UserId userId)
    {
        await using var dbConnection = new SqlConnection(_stringFactory.GetDefault());

        const string sql = """
                           SELECT Id, UserId, TourKey, Status
                           FROM [dbo].[ProductTourPostStatus]
                           WHERE UserId = @UserId
                           ORDER BY TourKey
                           """;

        var rows = await dbConnection.QueryAsync<ProductTourPostStatusDbRow>(sql, 
            new { UserId = userId.Value });
        
        return [.. rows.Select(MapFromDbRow)];
    }

    public async Task UpsertAsync(ProductTourPostStatus tourStatus)
    {
        await using var dbConnection = new SqlConnection(_stringFactory.GetDefault());

        const string sql = """
                           MERGE INTO [dbo].[ProductTourPostStatus] AS target
                           USING (SELECT @UserId AS UserId, @TourKey AS TourKey) AS source
                           ON target.UserId = source.UserId AND target.TourKey = source.TourKey
                           WHEN MATCHED THEN
                               UPDATE SET Status = @Status
                           WHEN NOT MATCHED THEN
                               INSERT (Id, UserId, TourKey, Status)
                               VALUES (@Id, @UserId, @TourKey, @Status);
                           """;

        await dbConnection.ExecuteAsync(sql, new
        {
            Id = tourStatus.Id,
            UserId = tourStatus.UserId.Value,
            TourKey = tourStatus.TourKey.Value,
            Status = (int)tourStatus.Status
        });
    }

    private static ProductTourPostStatus MapFromDbRow(ProductTourPostStatusDbRow row)
    {
        return new ProductTourPostStatus.ProductTourPostStatusBuilder()
            .WithId(row.Id)
            .WithUserId(new UserId(row.UserId))
            .WithTourKey(new TourKey(row.TourKey))
            .WithStatus((TourStatus)row.Status)
            .Build();
    }
}

