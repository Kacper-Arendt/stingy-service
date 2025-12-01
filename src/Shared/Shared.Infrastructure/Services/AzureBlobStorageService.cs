using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Shared.Abstractions.Services;

namespace Shared.Infrastructure.Services;

public class AzureBlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _baseUrl;
    private readonly string _connectionString;

    public AzureBlobStorageService(IConfiguration configuration)
    {
        _baseUrl = configuration["BlobStorage:BaseUrl"] ?? "";
        _connectionString = configuration["BlobStorage:ConnectionString"] ?? "";
        
        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException("Blob storage connection string is not set");
        
        _blobServiceClient = new BlobServiceClient(_connectionString);
    }

    public async Task<string> UploadAsync(Stream stream, string containerName, string fileName, string contentType)
    {
        try
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(stream, overwrite: true);
            
            var blobUrl = blobClient.Uri.ToString();
            
            return blobUrl;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            throw;
        }
    }

    public async Task DeleteAsync(string containerName, string fileName)
    {
        try
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string containerName, string fileName)
    {
        try
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var response = await blobClient.ExistsAsync();
            return response.Value;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            return false;
        }
    }

    public string GetBlobUrl(string containerName, string fileName)
    {
        return $"{_baseUrl.TrimEnd('/')}/{containerName}/{fileName}";
    }

    private async Task<BlobContainerClient> GetContainerClientAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
        
        return containerClient;
    }
}
