using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace AzureUploaderTransformerVideos
{

    public interface IAzureBlobsService
    {
        Task<string> UploadBlobFileAsync(string containerName, string fileName, string filePath);
        Task<bool> CheckBlobFileExistsAsync(string containerName, string fileName);
    }

    public class AzureBlobsService : IAzureBlobsService
    {
        private readonly AzureMediaServiceConfig azureMSConfig;
        private const string storageAccountKey = "DiSRgwmE1WZn8mCMXEi3tyxaMlTiFEDKHxkjYYfNMgbYzme4LVIRic4bq7GTHURBU/jSwBpPLggtK2Oi+J2Gtg==";

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
    }
}
