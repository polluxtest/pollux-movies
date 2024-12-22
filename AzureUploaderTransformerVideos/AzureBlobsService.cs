using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace AzureUploaderTransformerVideos
{

    public interface IAzureBlobsService
    {
        Task<string> UploadBlobFileAsync(string containerName, string fileName, string filePath);
        Task<bool> CheckBlobFileExistsAsync(string containerName, string fileName);
        Task<List<string>> GetAllFileNamesByContainerName(string containerName);
    }

    public class AzureBlobsService : IAzureBlobsService
    {
        private readonly AzureMediaServiceConfig azureMSConfig;
        private const string storageAccountKey = "d4wS4dKEXRu0w6cSUUDdRabMhiOhNNQchC74loMIktJ1f2g/7VngH4ch+8ZOo+thvFsvreKthRcU+AStUVUk2Q==";

        public AzureBlobsService(AzureMediaServiceConfig azureMSConfig)
        {
            this.azureMSConfig = azureMSConfig;
        }

        public async Task<string> UploadBlobFileAsync(string containerName, string fileName, string filePath)
        {
            var account = new CloudStorageAccount(new StorageCredentials(accountName: this.azureMSConfig.StorageAccountName, keyValue: storageAccountKey), true);
            var cloudBlobClient = account.CreateCloudBlobClient();
            var container = cloudBlobClient.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileName);
            await blob.UploadFromFileAsync(filePath);

            var blobUrl = blob.Uri.AbsoluteUri;

            return blobUrl.ToString();
        }

        public async Task<bool> CheckBlobFileExistsAsync(string containerName, string fileName)
        {
            var account = new CloudStorageAccount(new StorageCredentials(accountName: this.azureMSConfig.StorageAccountName, keyValue: storageAccountKey), true);
            var cloudBlobClient = account.CreateCloudBlobClient();
            var container = cloudBlobClient.GetContainerReference(containerName);

            try
            {
                var blob = await container.GetBlobReferenceFromServerAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<string>> GetAllFileNamesByContainerName(string containerName)
        {
            var storageConnectionString = this.azureMSConfig.StorageConnectionString;
            var blobServiceClient = new BlobServiceClient(storageConnectionString);

            var container = blobServiceClient.GetBlobContainerClient(containerName);

            var blobs =  container.GetBlobsAsync(Azure.Storage.Blobs.Models.BlobTraits.None);

            List<string> blobNames = new List<string>();

            await foreach(var blob in blobs)
            {
                var blobName = blob.Name;
                blobNames.Add(blobName);
            }

            return blobNames;
        }
    }
}
