using Microsoft.Extensions.Logging;
using Shared.Abstractions.Services;

namespace Shared.Infrastructure.FileService;

public interface IFileService
{
    Task<string> SaveImageAsync(Stream imageStream, string contentType, string fileName, string container);
    Task<bool> ValidateImageAsync(long fileSize, string? contentType);
}

public class FileService : IFileService
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ILogger<FileService> _logger;
    private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB

    private static readonly string[] AllowedImageContentTypes =
        ["image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml"];


    public FileService(IBlobStorageService blobStorageService, ILogger<FileService> logger)
    {
        _blobStorageService = blobStorageService;
        _logger = logger;
    }

    public async Task<string> SaveImageAsync(Stream imageStream, string contentType, string fileName, string container)
    {
        try
        {
            var imageUrl = await _blobStorageService.UploadAsync(imageStream, container, fileName, contentType);

            _logger.LogInformation("Successfully uploaded image: {ImageUrl}", imageUrl);

            return imageUrl;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            _logger.LogError(ex, "Failed to upload profile image {fileName} {container} {contentType}", fileName,
                container, contentType);
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

        if (!AllowedImageContentTypes.Contains(contentType?.ToLowerInvariant()))
        {
            _logger.LogWarning("Image content type {ContentType} is not allowed", contentType);
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
}