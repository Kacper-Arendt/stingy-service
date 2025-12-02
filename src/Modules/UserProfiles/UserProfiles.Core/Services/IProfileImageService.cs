using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Services;

public interface IProfileImageService
{
    Task<string> UploadProfileImageAsync(Stream imageStream, UserId userId, string fileName, string contentType);
    Task DeleteProfileImageAsync(string imageUrl);
    Task<bool> ValidateImageAsync(long fileSize, string? contentType);
}
