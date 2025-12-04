using Microsoft.Extensions.Caching.Memory;
using Microsoft.Data.SqlClient;
using Dapper;
using UserProfiles.Shared.Services;
using UserProfiles.Shared.Dtos;
using UserProfiles.Core.Repositories;
using Shared.Abstractions.ValueObjects;
using Shared.Abstractions.Factories;

namespace UserProfiles.Core.Services;

public class UserDisplayService : IUserDisplayService
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IDbConnectionFactory _factory;
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());
    
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private const string CacheKeyPrefix = "user_display:";

    public UserDisplayService(
        IUserProfileRepository userProfileRepository,
        IDbConnectionFactory factory)
    {
        _userProfileRepository = userProfileRepository;
        _factory = factory;
    }

    public async Task<UserDisplayInfoDto?> GetUserDisplayInfoAsync(UserId userId)
    {
        var cacheKey = $"{CacheKeyPrefix}{userId.Value}";
        
        if (_cache.TryGetValue(cacheKey, out UserDisplayInfoDto? cachedResult))
        {
            return cachedResult;
        }

        var result = await GetUserDisplayInfoFromDatabase(userId);
        
        if (result != null)
        {
            _cache.Set(cacheKey, result, CacheDuration);
        }

        return result;
    }

    public async Task<List<UserDisplayInfoDto>> GetUsersDisplayInfoAsync(IEnumerable<UserId> userIds)
    {
        var userIdsList = userIds.ToList();
        var results = new List<UserDisplayInfoDto>();
        var uncachedUserIds = new List<UserId>();

        foreach (var userId in userIdsList)
        {
            var cacheKey = $"{CacheKeyPrefix}{userId.Value}";
            if (_cache.TryGetValue(cacheKey, out UserDisplayInfoDto? cachedResult) && cachedResult != null)
            {
                results.Add(cachedResult);
            }
            else
            {
                uncachedUserIds.Add(userId);
            }
        }

        if (uncachedUserIds.Any())
        {
            var uncachedResults = await GetUsersDisplayInfoFromDatabase(uncachedUserIds);
            
            foreach (var result in uncachedResults)
            {
                var cacheKey = $"{CacheKeyPrefix}{result.UserId}";
                _cache.Set(cacheKey, result, CacheDuration);
                results.Add(result);
            }
        }

        return results;
    }

    private async Task<UserDisplayInfoDto?> GetUserDisplayInfoFromDatabase(UserId userId)
    {
        var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
        
        if (userProfile != null)
        {
            return new UserDisplayInfoDto
            {
                UserId = userProfile.UserId.Value,
                DisplayName = userProfile.DisplayName.Value,
                ShortDisplayName = UserDisplayInfoDto.GenerateShortDisplayName(userProfile.DisplayName.Value),
                ProfileImageUrl = userProfile.ProfileImageUrl?.Value,
                HasProfile = true
            };
        }

        var userEmail = await GetUserEmailFromAuth(userId);
        if (userEmail != null)
        {
            var displayName = userEmail.Split('@')[0];
            
            return new UserDisplayInfoDto
            {
                UserId = userId.Value,
                DisplayName = displayName,
                ShortDisplayName = UserDisplayInfoDto.GenerateShortDisplayName(displayName),
                ProfileImageUrl = null,
                HasProfile = false
            };
        }

        return null;
    }

    private async Task<List<UserDisplayInfoDto>> GetUsersDisplayInfoFromDatabase(List<UserId> userIds)
    {
        var results = new List<UserDisplayInfoDto>();

        foreach (var userId in userIds)
        {
            var result = await GetUserDisplayInfoFromDatabase(userId);
            if (result != null)
            {
                results.Add(result);
            }
        }

        return results;
    }

    private async Task<string?> GetUserEmailFromAuth(UserId userId)
    {
        await using var dbConnection = new SqlConnection(_factory.GetDefault());
        
        const string sql = """
                           SELECT Email 
                           FROM [auth].[AspNetUsers] 
                           WHERE Id = @UserId
                           """;
        
        return await dbConnection.QuerySingleOrDefaultAsync<string?>(sql, new { UserId = userId.Value });
    }
}
