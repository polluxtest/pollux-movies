using System;
using System.Linq;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace CopyExistingBlobIntoAsset
{


    //    {
    //  "AadClientId": "c5d6b4bb-0bfe-4352-93b6-553f56a50970",
    //  "AadEndpoint": "https://login.microsoftonline.com",
    //  "AadSecret": "Lirk4wg.4W_6c7dzWEHOAy~6qK.W--MmyI",
    //  "AadTenantId": "d436ce91-8288-496c-bffd-3ef3bf69d53f",
    //  "AccountName": "polluxmediaservices",
    //  "ArmAadAudience": "https://management.core.windows.net/",
    //  "ArmEndpoint": "https://management.azure.com/",
    //  "Location": "West US 2",
    //  "ResourceGroup": "develop",
    //  "SubscriptionId": "fbc08398-07e5-4947-8de5-e02636208344"
    //}
    class Program
    {
        // Read values from the App.config file.
        private static readonly string _sourceStorageAccountName = "polluxstorageaccount";
        private static readonly string _sourceStorageAccountKey = "Nhyc9bYSC8X4sAuGduO+euvfjlPsRkz5qCBnAKNzIvBhENQRjQABjrzOHl5J1cNHzc8dmHdfOueuaPQay/KZDA==";
        private static readonly string _NameOfBlobContainerYouWantToCopy = "polluxmoviescontainer";
        private static readonly string _AMSAADTenantDomain = "samgoldmrambiguoushotmail.onmicrosoft.com";
        private static readonly string _AMSRESTAPIEndpoint = "https://management.azure.com/";
        private static readonly string _AMSClientId = "c5d6b4bb-0bfe-4352-93b6-553f56a50970";
        private static readonly string _AMSClientSecret = "N68oDCHBMzV3nwOwwu55--Fvcf4a_6.z3v";
        private static readonly string _AMSStorageAccountName = "polluxstorageaccount";
        private static readonly string _AMSStorageAccountKey = "Nhyc9bYSC8X4sAuGduO+euvfjlPsRkz5qCBnAKNzIvBhENQRjQABjrzOHl5J1cNHzc8dmHdfOueuaPQay/KZDA==";

        // Field for service context.
        private static CloudMediaContext _context = null;
        private static CloudStorageAccount _sourceStorageAccount = null;
        private static CloudStorageAccount _destinationStorageAccount = null;
        static void Main(string[] args)
        {

            AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(_AMSAADTenantDomain, AzureEnvironments.AzureCloudEnvironment);

            var tokenProvider = new AzureAdTokenProvider(tokenCredentials);

            // Create the context for your source Media Services account.
            _context = new CloudMediaContext(new Uri(_AMSRESTAPIEndpoint), tokenProvider);

            var assets = _context.Assets;

            foreach (var a in assets)
            {
                Console.WriteLine(a.Name);
            }

            _sourceStorageAccount = new CloudStorageAccount(new StorageCredentials(_sourceStorageAccountName, _sourceStorageAccountKey), true);

            _destinationStorageAccount = new CloudStorageAccount(new StorageCredentials(_AMSStorageAccountName, _AMSStorageAccountKey), true);

            CloudBlobClient sourceCloudBlobClient = _sourceStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer sourceContainer = sourceCloudBlobClient.GetContainerReference(_NameOfBlobContainerYouWantToCopy);

            CreateAssetFromExistingBlobs(sourceContainer);

            Console.WriteLine("Done");
        }

        static private IAsset CreateAssetFromExistingBlobs(CloudBlobContainer sourceBlobContainer)
        {
            CloudBlobClient destBlobStorage = _destinationStorageAccount.CreateCloudBlobClient();

            // Create a new asset. 
            //string assetName = " Catch.Me.If.You.Can.2002.720p.BluRay.x264.YIFY.mp4";

            IAsset asset = _context.Assets.Create("NewAsset_" + Guid.NewGuid(), AssetCreationOptions.None);

            IAccessPolicy writePolicy = _context.AccessPolicies.Create("writePolicy",
                TimeSpan.FromHours(24), AccessPermissions.Write);

            ILocator destinationLocator =
                _context.Locators.CreateLocator(LocatorType.Sas, asset, writePolicy);

            // Get the asset container URI and Blob copy from mediaContainer to assetContainer. 
            CloudBlobContainer destAssetContainer =
                destBlobStorage.GetContainerReference((new Uri(destinationLocator.Path)).Segments[1]);

            if (destAssetContainer.CreateIfNotExists())
            {
                destAssetContainer.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }

            var blobList = sourceBlobContainer.ListBlobs();

            foreach (CloudBlockBlob sourceBlob in blobList)
            {
                var assetFile = asset.AssetFiles.Create((sourceBlob as ICloudBlob).Name);

                ICloudBlob destinationBlob = destAssetContainer.GetBlockBlobReference(assetFile.Name);

                CopyBlob(sourceBlob, destAssetContainer);

                sourceBlob.FetchAttributes();
                assetFile.ContentFileSize = (sourceBlob as ICloudBlob).Properties.Length;
                assetFile.Update();
                Console.WriteLine("File {0} is of {1} size", assetFile.Name, assetFile.ContentFileSize);
            }

            asset.Update();

            destinationLocator.Delete();
            writePolicy.Delete();

            // Set the primary asset file.
            // If, for example, we copied a set of Smooth Streaming files, 
            // set the .ism file to be the primary file. 
            // If we, for example, copied an .mp4, then the mp4 would be the primary file. 
            var ismAssetFile = asset.AssetFiles.ToList().
                Where(f => f.Name.EndsWith(".ism", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            // The following code assigns the first .ism file as the primary file in the asset.
            // An asset should have one .ism file.  
            if (ismAssetFile != null)
            {
                ismAssetFile.IsPrimary = true;
                ismAssetFile.Update();
            }

            return asset;
        }

        /// <summary>
        /// Copies the specified blob into the specified container.
        /// </summary>
        /// <param name="sourceBlob">The source container.</param>
        /// <param name="destinationContainer">The destination container.</param>
        static private void CopyBlob(ICloudBlob sourceBlob, CloudBlobContainer destinationContainer)
        {
            var signature = sourceBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24)
            });

            var destinationBlob = destinationContainer.GetBlockBlobReference(sourceBlob.Name);

            if (destinationBlob.Exists())
            {
                Console.WriteLine(string.Format("Destination blob '{0}' already exists. Skipping.", destinationBlob.Uri));
            }
            else
            {

                // Display the size of the source blob.
                Console.WriteLine(sourceBlob.Properties.Length);

                Console.WriteLine(string.Format("Copy blob '{0}' to '{1}'", sourceBlob.Uri, destinationBlob.Uri));
                destinationBlob.StartCopy(new Uri(sourceBlob.Uri.AbsoluteUri + signature));

                while (true)
                {
                    // The StartCopyFromBlob is an async operation, 
                    // so we want to check if the copy operation is completed before proceeding. 
                    // To do that, we call FetchAttributes on the blob and check the CopyStatus. 
                    destinationBlob.FetchAttributes();
                    if (destinationBlob.CopyState.Status != CopyStatus.Pending)
                    {
                        break;
                    }
                    //It's still not completed. So wait for some time.
                    System.Threading.Thread.Sleep(1000);
                }

                // Display the size of the destination blob.
                Console.WriteLine(destinationBlob.Properties.Length);

            }



        }
    }

}
