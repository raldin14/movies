using Azure.Storage.Blobs;

namespace MovieAPI.Utils;

public class StorageAzureStorage : IStorageAzureStorage
{
    private string _connectionString;
    public StorageAzureStorage(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("AzureStorage");
    }

    public async Task<string> SaveFile(string container, IFormFile file)
    {
        var client = new BlobContainerClient(_connectionString, container);
        await client.CreateIfNotExistsAsync();
        await client.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
        
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var blobClient = client.GetBlobClient(fileName);
        await blobClient.UploadAsync(file.OpenReadStream());
        return blobClient.Uri.ToString();
    }

    public async Task DeleteFile(string container, string route)
    {
        if (string.IsNullOrEmpty(route))
        {
            return;
        }
        
        var client = new BlobContainerClient(_connectionString, container);
        await client.CreateIfNotExistsAsync();
        
        var file =  Path.GetFileName(route);
        var blobClient = client.GetBlobClient(file);
        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<string> EditFile(string container, string route, IFormFile file)
    {
        await DeleteFile(container, route);
        return await SaveFile(container, file);
    }
}