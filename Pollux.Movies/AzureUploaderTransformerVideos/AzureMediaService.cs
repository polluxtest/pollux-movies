using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using Movies.Application;
using Movies.Domain.Entities;

namespace AzureUploaderTransformerVideos
{
    public class AzureMediaService
    {
        private const string AdaptiveStreamingTransformName = "polluxmediaservicesencodingtransform";
        private const string VideoStoragePath = @"W:\pollux\newMovies";
        private readonly IMoviesService _moviesService;
        private readonly AzureMediaServiceConfig azureMSConfig;

        public AzureMediaService(
             IMoviesService moviesService,
             AzureMediaServiceConfig azureMsConfig)
        {
            this._moviesService = moviesService;
            this.azureMSConfig = azureMsConfig;
        }

        /// <summary>
        /// Runs the asynchronous process of uploading transforming and encoding a video.
        /// </summary>
        /// /// <returns>Task.</returns>
        public async Task RunAsync()
        {
            try
            {
                var movies = await this._moviesService.GetAll();

                foreach (var movie in movies)
                {
                    await this.ProcessAsync(movie);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Processes the asynchronous.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns>Task.</returns>
        private async Task ProcessAsync(Movie movie)
        {
            Debug.WriteLine("starting creating assents.");

            IAzureMediaServicesClient azureMediaServiceClient = await this.CreateMediaServicesClientAsync(this.azureMSConfig);

            azureMediaServiceClient.LongRunningOperationRetryTimeout = 2;

            var amsIdentity = this.GetAzureVideoAssetIdentity(movie);

            var transform = await this.GetOrCreateTransformAsync(azureMediaServiceClient);

            var inputAsset = await this.CreateInputAssetAsync(azureMediaServiceClient, amsIdentity);

            _ = new JobInputAsset(assetName: amsIdentity.OutputAssetName);

            var outputAsset = await this.CreateOutputAssetAsync(azureMediaServiceClient, amsIdentity.OutputAssetName);

            await this.SubmitJobAsync(azureMediaServiceClient, amsIdentity);

            Job job = await this.WaitForJobToFinishAsync(azureMediaServiceClient, amsIdentity.JobName);

            if (job.State == JobState.Finished)
            {
                this.UpdateMovie(azureMediaServiceClient, movie, amsIdentity);
                Debug.WriteLine("Done Job The streaming url " + movie.UrlVideo);
            }

            await this.CleanUpAsync(azureMediaServiceClient, amsIdentity.JobName);

            Debug.WriteLine("Done clean up job and input asset");
        }

        /// <summary>
        /// Creates the input asset asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="assetModel">The asset identity.</param>
        /// <returns>Asset.</returns>
        /// <exception cref="System.IO.FileNotFoundException">IF the file to upload does not exists.</exception>
        private async Task<Asset> CreateInputAssetAsync(
            IAzureMediaServicesClient client,
            AzureVideoAssetModel assetModel)
        {
            Asset asset = await client.Assets.CreateOrUpdateAsync(this.azureMSConfig.ResourceGroup, this.azureMSConfig.AccountName, assetModel.InputAssetName, new Asset());

            var response = await client.Assets.ListContainerSasAsync(
                this.azureMSConfig.ResourceGroup,
                this.azureMSConfig.AccountName,
                assetModel.InputAssetName,
                permissions: AssetContainerPermission.ReadWrite,
                expiryTime: DateTime.UtcNow.AddHours(4).ToUniversalTime());

            var sasUri = new Uri(response.AssetContainerSasUrls.First());

            BlobContainerClient container = new BlobContainerClient(sasUri);
            BlobClient blob = container.GetBlobClient(Path.GetFileName(assetModel.FileName));

            this.UploadVideoToAzure(blob, assetModel);

            return asset;
        }

        /// <summary>
        /// Creates an output asset. The output from the encoding Job must be written to an Asset.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="outputAssetName">The output asset name.</param>
        /// <returns>Asset.</returns>
        // <CreateOutputAsset>
        private async Task<Asset> CreateOutputAssetAsync(
            IAzureMediaServicesClient client,
            string outputAssetName)
        {
            // Check if an Asset already exists
            Asset inputAsset = await client.Assets.GetAsync(this.azureMSConfig.ResourceGroup, this.azureMSConfig.AccountName, outputAssetName);
            Asset outputAsset = new Asset();

            if (inputAsset != null)
            {
                throw new ArgumentException("the output asset name already exists", outputAssetName);
            }

            return await client.Assets.CreateOrUpdateAsync(this.azureMSConfig.ResourceGroup, this.azureMSConfig.AccountName, outputAssetName, outputAsset);
        }

        /// <summary>
        /// If the specified transform exists, get that transform.
        /// If the it does not exist, creates a new transform with the specified output. 
        /// In this case, the output is set to encode a video using one of the built-in encoding presets.
        /// </summary>
        /// <param name="azureMediaServiceClient">The Media Services client.</param>
        /// <returns>Transform</returns>
        private async Task<Transform> GetOrCreateTransformAsync(
            IAzureMediaServicesClient azureMediaServiceClient)
        {
            // Does a Transform already exist with the desired name? Assume that an existing Transform with the desired name
            // also uses the same recipe or Preset for processing content.
            Transform transform = await azureMediaServiceClient.Transforms.
                GetAsync(this.azureMSConfig.ResourceGroup, this.azureMSConfig.AccountName, AdaptiveStreamingTransformName);

            if (transform == null)
            {
                throw new ArgumentException("could not find the specific transformer ", AdaptiveStreamingTransformName);
            }

            return transform;
        }

        /// <summary>
        /// Submits a request to Media Services to apply the specified Transform to a given input video.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        private async Task<Job> SubmitJobAsync(
            IAzureMediaServicesClient client,
            AzureVideoAssetModel assetModel)
        {
            // Use the name of the created input asset to create the job input.
            JobInput jobInput = new JobInputAsset(assetName: assetModel.InputAssetName);

            JobOutput[] jobOutputs =
            {
                 new JobOutputAsset(assetModel.OutputAssetName),
            };

            Job job = await client.Jobs.CreateAsync(
                this.azureMSConfig.ResourceGroup,
                this.azureMSConfig.AccountName,
                AdaptiveStreamingTransformName,
                assetModel.JobName,
                new Job
                {
                    Input = jobInput,
                    Outputs = jobOutputs,
                });

            return job;
        }

        /// <summary>
        /// Polls Media Services for the status of the Job.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="jobName">The name of the job you submitted.</param>
        /// <returns>Job.</returns>
        // <WaitForJobToFinish>
        private async Task<Job> WaitForJobToFinishAsync(
            IAzureMediaServicesClient client,
            string jobName)
        {
            const int SleepIntervalMs = 20 * 1000;

            Job job;
            do
            {
                job = await client.Jobs.GetAsync(this.azureMSConfig.ResourceGroup, this.azureMSConfig.AccountName, AdaptiveStreamingTransformName, jobName);

                Debug.WriteLine($"Job is '{job.State}'.");
                for (int i = 0; i < job.Outputs.Count; i++)
                {
                    JobOutput output = job.Outputs[i];
                    Debug.Write($"\tJobOutput[{i}] is '{output.State}'.");
                    if (output.State == JobState.Processing)
                    {
                        Debug.Write($"  Progress (%): '{output.Progress}'.");
                    }

                    Debug.WriteLine(string.Empty);
                }

                if (job.State != JobState.Finished && job.State != JobState.Error && job.State != JobState.Canceled)
                {
                    await Task.Delay(SleepIntervalMs);
                }
            }
            while (job.State != JobState.Finished && job.State != JobState.Error && job.State != JobState.Canceled);

            return job;
        }

        /// <summary>
        /// Creates a StreamingLocator for the specified asset and with the specified streaming policy name.
        /// Once the StreamingLocator is created the output asset is available to clients for playback.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <returns>StreamingLocator.</returns>
        private async Task<StreamingLocator> CreateStreamingLocatorAsync(
            IAzureMediaServicesClient client,
            AzureMediaServiceConfig amsConfig,
            AzureVideoAssetModel azureVideoAsset)
        {
            StreamingLocator locator = await client.StreamingLocators.CreateAsync(
                amsConfig.ResourceGroup,
                amsConfig.AccountName,
                azureVideoAsset.LocatorName,
                new StreamingLocator
                {
                    AssetName = azureVideoAsset.OutputAssetName,
                    StreamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly,
                });

            return locator;
        }

        /// <summary>
        /// Checks if the "default" streaming endpoint is in the running state,
        /// if not, starts it.
        /// Then, builds the streaming URLs.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="locatorName">The name of the StreamingLocator that was created.</param>
        /// <returns>Url Vide.</returns>
        // <GetStreamingURLs>
        private async Task<string> GetStreamingUrlsAsync(
            IAzureMediaServicesClient client,
            string resourceGroupName,
            string accountName,
            string locatorName)
        {
            const string DefaultStreamingEndpointName = "default";

            IList<string> streamingUrls = new List<string>();

            StreamingEndpoint streamingEndpoint = await client.StreamingEndpoints.GetAsync(resourceGroupName, accountName, DefaultStreamingEndpointName);

            if (streamingEndpoint != null)
            {
                if (streamingEndpoint.ResourceState != StreamingEndpointResourceState.Running)
                {
                    await client.StreamingEndpoints.StartAsync(resourceGroupName, accountName, DefaultStreamingEndpointName);
                }
            }

            ListPathsResponse paths = await client.StreamingLocators.ListPathsAsync(resourceGroupName, accountName, locatorName);

            foreach (StreamingPath path in paths.StreamingPaths)
            {
                UriBuilder uriBuilder = new UriBuilder
                {
                    Scheme = "https",
                    Host = streamingEndpoint.HostName,
                    Path = path.Paths[0],
                };
                streamingUrls.Add(uriBuilder.ToString());
            }

            return streamingUrls[2];
        }

        /// <summary>
        /// Cleans up asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="jobName">Name of the job.</param>
        private async Task CleanUpAsync(
           IAzureMediaServicesClient client,
           string jobName)
        {
            await client.Jobs.DeleteAsync(this.azureMSConfig.ResourceGroup, this.azureMSConfig.AccountName, AdaptiveStreamingTransformName, jobName);
        }

        /// <summary>
        /// Uploads the video.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <param name="assetModel">The asset identity.</param>
        /// <exception cref="System.IO.FileNotFoundException">File not found.</exception>
        private void UploadVideoToAzure(BlobClient blob, AzureVideoAssetModel assetModel)
        {
            var videoLocation = Path.Combine(VideoStoragePath, assetModel.FileName);

            if (!File.Exists(videoLocation))
            {
                throw new FileNotFoundException();
            }

            Progress<long> progress = new Progress<long>();
            progress.ProgressChanged += this.Progress_ProgressChanged; // register process event to check status.

            var file = File.ReadAllBytes(videoLocation);
            var stream = new MemoryStream(file);

            Debug.WriteLine("starting uploading... " + assetModel.Name);

            blob.Upload(stream, null, null, null, progress, null);

            Debug.WriteLine("finished uploading " + assetModel.Name);
        }

        /// <summary>
        /// Updates the movie.
        /// </summary>
        /// <param name="azureMediaServiceClient">The azure media service client.</param>
        /// <param name="movie">The movie.</param>
        /// <param name="amsIdentity">The ams identity.</param>
        private async void UpdateMovie(IAzureMediaServicesClient azureMediaServiceClient, Movie movie, AzureVideoAssetModel amsIdentity)
        {
            StreamingLocator locator = await this.CreateStreamingLocatorAsync(azureMediaServiceClient, this.azureMSConfig, amsIdentity);

            movie.UrlVideo = await this.GetStreamingUrlsAsync(azureMediaServiceClient, this.azureMSConfig.ResourceGroup, this.azureMSConfig.AccountName, locator.Name);
            movie.ProcessedByAzureJob = true;
            await this._moviesService.UpdateMovie(movie);
        }

        /// <summary>
        /// Gets the azure video asset identity.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns>AzureVideoAssetModel.</returns>
        private AzureVideoAssetModel GetAzureVideoAssetIdentity(Movie movie)
        {
            var name = movie.Name;

            return new AzureVideoAssetModel()
            {
                Name = name,
                FileName = movie.FileName,
                LocatorName = $"{name}-locator",
                InputAssetName = $"{name}-input",
                OutputAssetName = $"{name}-output",
                JobName = $"{name}-job",
            };
        }

        /// <summary>
        /// Create the ServiceClientCredentials object based on the credentials
        /// supplied in local configuration file.
        /// </summary>
        /// <param name="config">The param is of type ConfigWrapper. This class reads values from local configuration file.</param>
        /// <returns>ServiceClientCredentials.</returns>
        private async Task<ServiceClientCredentials> GetCredentialsAsync(AzureMediaServiceConfig config)
        {
            // Use ApplicationTokenProvider.LoginSilentWithCertificateAsync or UserTokenProvider.LoginSilentAsync to get a token using service principal with certificate
            //// ClientAssertionCertificate
            //// ApplicationTokenProvider.LoginSilentWithCertificateAsync

            // Use ApplicationTokenProvider.LoginSilentAsync to get a token using a service principal with symetric key
            ClientCredential clientCredential = new ClientCredential(config.AadClientId, config.AadSecret);
            return await ApplicationTokenProvider.LoginSilentAsync(config.AadTenantId, clientCredential, ActiveDirectoryServiceSettings.Azure);
        }

        /// <summary>
        /// Creates the AzureMediaServicesClient object based on the credentials
        /// supplied in local configuration file.
        /// </summary>
        /// <param name="config">The param is of type ConfigWrapper. This class reads values from local configuration file.</param>
        /// <returns>IAzureMediaServicesClient.</returns>
        // <CreateMediaServicesClient>
        private async Task<IAzureMediaServicesClient> CreateMediaServicesClientAsync(AzureMediaServiceConfig config)
        {
            var credentials = await this.GetCredentialsAsync(config);

            return new AzureMediaServicesClient(config.ArmEndpoint, credentials)
            {
                SubscriptionId = config.SubscriptionId,
            };
        }

        /// <summary>
        /// Progresses the progress changed when uploading the video.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="progress">The progress.</param>
        private void Progress_ProgressChanged(object sender, long progress)
        {
            Debug.WriteLine(progress);
        }
    }
}
