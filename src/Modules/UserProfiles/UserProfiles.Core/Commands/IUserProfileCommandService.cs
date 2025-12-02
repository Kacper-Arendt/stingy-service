using UserProfiles.Core.Commands.Dtos;
using UserProfiles.Core.Queries.Dtos;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Commands;

public interface IUserProfileCommandService
{
    Task<UserProfileDto> CreateAsync(UserId userId, CreateUserProfileDto dto);
    Task<UserProfileDto> UpdateAsync(UserId userId, UpdateUserProfileDto dto);
    Task<UserProfileDto> CreateFromEmailAsync(UserId userId, Email email);
}
