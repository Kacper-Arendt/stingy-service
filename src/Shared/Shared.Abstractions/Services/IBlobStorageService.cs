
namespace Shared.Abstractions.Services;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream stream, string containerName, string fileName, string contentType);
    Task DeleteAsync(string containerName, string fileName);
    Task<bool> ExistsAsync(string containerName, string fileName);
    string GetBlobUrl(string containerName, string fileName);
}
