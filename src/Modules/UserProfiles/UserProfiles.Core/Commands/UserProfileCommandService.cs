using UserProfiles.Core.Commands.Dtos;
using UserProfiles.Core.Queries.Dtos;
using UserProfiles.Core.Repositories;
using UserProfiles.Domain.Entities;
using UserProfiles.Domain.ValueObjects;
using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Commands;

public class UserProfileCommandService : IUserProfileCommandService
{
    private readonly IUserProfileRepository _userProfileRepository;

    public UserProfileCommandService(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<UserProfileDto> CreateAsync(UserId userId, CreateUserProfileDto dto)
    {
        if (await _userProfileRepository.ExistsAsync(userId))
            throw new DomainModelArgumentException($"User profile for user {userId.Value} already exists.");

        var displayName = new DisplayName(dto.DisplayName);

        if (await _userProfileRepository.IsDisplayNameTakenAsync(displayName))
            throw new DomainModelArgumentException($"Display name '{dto.DisplayName}' is already taken.");

        var userProfile = new UserProfile.UserProfileBuilder()
            .WithUserId(userId)
            .WithDisplayName(displayName)
            .WithProfileImageUrl(string.IsNullOrWhiteSpace(dto.ProfileImageUrl)
                ? null
                : new ProfileImageUrl(dto.ProfileImageUrl))
            .WithBio(string.IsNullOrWhiteSpace(dto.Bio) ? null : new UserBio(dto.Bio))
            .WithTimeZone(new DateTimeZone(dto.TimeZone))
            .WithVisibility(dto.Visibility)
            .Build();

        await _userProfileRepository.SaveAsync(userProfile);

        return MapToDto(userProfile);
    }

    public async Task<UserProfileDto> UpdateAsync(UserId userId, UpdateUserProfileDto dto)
    {
        var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
        if (userProfile == null)
            throw new DomainModelArgumentException($"User profile for user {userId.Value} not found.");

        var displayName = new DisplayName(dto.DisplayName);

        if (displayName.Value != userProfile.DisplayName.Value &&
            await _userProfileRepository.IsDisplayNameTakenAsync(displayName, userId))
            throw new DomainModelArgumentException($"Display name '{dto.DisplayName}' is already taken.");

        userProfile.UpdateProfile(
            displayName,
            string.IsNullOrWhiteSpace(dto.ProfileImageUrl) ? null : new ProfileImageUrl(dto.ProfileImageUrl),
            string.IsNullOrWhiteSpace(dto.Bio) ? null : new UserBio(dto.Bio),
            new DateTimeZone(dto.TimeZone),
            dto.Visibility
        );

        await _userProfileRepository.UpdateAsync(userProfile);

        return MapToDto(userProfile);
    }


    public async Task<UserProfileDto> CreateFromEmailAsync(UserId userId, Email email)
    {
        if (await _userProfileRepository.ExistsAsync(userId))
            throw new DomainModelArgumentException($"User profile for user {userId.Value} already exists.");

        var userProfile = UserProfile.CreateFromEmail(userId, email);

        var baseDisplayName = userProfile.DisplayName.Value;
        var finalDisplayName = userProfile.DisplayName;
        var counter = 1;

        while (await _userProfileRepository.IsDisplayNameTakenAsync(finalDisplayName))
        {
            finalDisplayName = new DisplayName($"{baseDisplayName}{counter}");
            counter++;
        }

        if (finalDisplayName.Value != baseDisplayName)
        {
            userProfile.UpdateProfile(
                finalDisplayName,
                userProfile.ProfileImageUrl,
                userProfile.Bio,
                userProfile.TimeZone,
                userProfile.Visibility
            );
        }

        await _userProfileRepository.SaveAsync(userProfile);

        return MapToDto(userProfile);
    }

    private static UserProfileDto MapToDto(UserProfile userProfile)
    {
        return new UserProfileDto
        {
            UserId = userProfile.UserId.Value,
            DisplayName = userProfile.DisplayName.Value,
            ProfileImageUrl = userProfile.ProfileImageUrl?.Value,
            Bio = userProfile.Bio?.Value,
            TimeZone = userProfile.TimeZone.Value,
            Visibility = userProfile.Visibility,
            CreatedAt = userProfile.CreatedAt.Value,
            LastModifiedAt = userProfile.LastModifiedAt.Value
        };
    }
}