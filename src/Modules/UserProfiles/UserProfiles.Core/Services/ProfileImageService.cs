using Microsoft.Extensions.Logging;
using Shared.Abstractions.Services;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Services;

public class ProfileImageService : IProfileImageService
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ILogger<ProfileImageService> _logger;
    
    private const string ContainerName = "retrospectacle-profile-images";
    private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml"];

    public ProfileImageService(IBlobStorageService blobStorageService, ILogger<ProfileImageService> logger)
    {
        _blobStorageService = blobStorageService;
        _logger = logger;
    }

    public async Task<string> UploadProfileImageAsync(Stream imageStream, UserId userId, string fileName, string contentType)
    {
        var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
        var finalFileName = $"{userId.Value}{fileExtension}";

        try
        {
            var imageUrl = await _blobStorageService.UploadAsync(imageStream, ContainerName, finalFileName, contentType);
            
            _logger.LogInformation("Successfully uploaded profile image for user {UserId}: {ImageUrl}", userId.Value, imageUrl);
            
            return imageUrl;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            _logger.LogError(ex, "Failed to upload profile image for user {UserId}", userId.Value);
            throw;
        }
    }

    public async Task DeleteProfileImageAsync(string imageUrl)
    {
        try
        {
            var uri = new Uri(imageUrl);
            var fileName = Path.GetFileName(uri.LocalPath);
            
            await _blobStorageService.DeleteAsync(ContainerName, fileName);
            
            _logger.LogInformation("Successfully deleted profile image: {ImageUrl}", imageUrl);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            _logger.LogError(ex, "Failed to delete profile image: {ImageUrl}", imageUrl);
            throw;
        }
    }

    public Task<bool> ValidateImageAsync(long fileSize, string? contentType)
    {
        if (fileSize <= 0)
        {
            _logger.LogWarning("Image file size is zero or negative");
            return Task.FromResult(false);
        }

        if (fileSize > MaxFileSizeBytes)
        {
            _logger.LogWarning("Image file size {Size} exceeds maximum allowed size {MaxSize}", fileSize, MaxFileSizeBytes);
            return Task.FromResult(false);
        }

        if (!AllowedContentTypes.Contains(contentType?.ToLowerInvariant()))
        {
            _logger.LogWarning("Image content type {ContentType} is not allowed", contentType);
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
}
