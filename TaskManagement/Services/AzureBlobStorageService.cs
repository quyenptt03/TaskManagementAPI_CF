using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using TaskManagement.Interfaces;

namespace TaskManagement.Services
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        //private readonly BlobServiceClient blobServiceClient = new BlobServiceClient("your_connection_string");

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureBlobStorage:ConnectionString"];
            var containerName = configuration["AzureBlobStorage:ContainerName"];
            _containerClient = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            string fileName = $"{Guid.NewGuid()}_{file.FileName}";
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            return blobClient.Uri.ToString();
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            Uri uri = new Uri(fileUrl);
            string blobName = Path.GetFileName(uri.LocalPath);
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            return await blobClient.DeleteIfExistsAsync();
        }
    }
}
