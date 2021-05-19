﻿using System;
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
        private readonly IMovieAzureAssetsService movieAzureAssetsService;
        private readonly IMovieService movieService;
        private readonly AzureMediaServiceConfig amsConfig;
        private const string AdaptiveStreamingTransformName = "polluxmediaservicesencodingtransform";
        private const string VideoStoragePath = @"Movies";

        public AzureMediaService(
             IMovieAzureAssetsService movieAzureAssetsService,
             IMovieService movieService,
             AzureMediaServiceConfig amsConfig)
        {
            this.movieAzureAssetsService = movieAzureAssetsService;
            this.movieService = movieService;
            this.amsConfig = amsConfig;
        }

        /// <summary>
        /// Runs the process of video process and uploading.
        /// </summary>
        public async Task RunAsync()
        {
            try
            {
                var movies = await this.movieService.GetAll();

                foreach (var movie in movies)
                {
                    await ProcessAsync(movie);
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
        /// <returns></returns>
        private async Task ProcessAsync(Movie movie)
        {
            Debug.WriteLine("starting creating assents.");
            IAzureMediaServicesClient azureMediaServiceClient = await CreateMediaServicesClientAsync(amsConfig);

            azureMediaServiceClient.LongRunningOperationRetryTimeout = 2;

            var amsIdentity = this.GetAzureVideoAssetIdentity(movie);

            var transform = await GetOrCreateTransformAsync(azureMediaServiceClient, amsConfig);

            var inputAsset = await CreateInputAssetAsync(azureMediaServiceClient, amsConfig, amsIdentity);

            _ = new JobInputAsset(assetName: amsIdentity.OutputAssetName);

            var outputAsset = await CreateOutputAssetAsync(azureMediaServiceClient, amsConfig, amsIdentity.OutputAssetName);

            _ = await SubmitJobAsync(azureMediaServiceClient, amsConfig, amsIdentity);

            Job job = await WaitForJobToFinishAsync(azureMediaServiceClient, amsConfig, amsIdentity.JobName);


            if (job.State == JobState.Finished)
            {
                StreamingLocator locator = await CreateStreamingLocatorAsync(azureMediaServiceClient, amsConfig, amsIdentity);
                movie.Url = await GetStreamingUrlsAsync(azureMediaServiceClient, amsConfig.ResourceGroup, amsConfig.AccountName, locator.Name);

                await this.movieService.Update(movie);
                await this.movieAzureAssetsService.Create(movie.Id, amsIdentity.OutputAssetName, amsIdentity.InputAssetName);

                Debug.WriteLine("Done Job The streaming url " + movie.Url);
            }

            await CleanUpAsync(azureMediaServiceClient, amsConfig, amsIdentity.JobName);
        }

        /// <summary>
        /// Gets the azure video asset identity.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns></returns>
        private AzureVideoAssetIdentity GetAzureVideoAssetIdentity(Movie movie)
        {
            var name = movie.Name;

            return new AzureVideoAssetIdentity()
            {
                Name = name,
                FileName = movie.FileName,
                LocatorName = $"{name}-locator",
                InputAssetName = $"{name}-input",
                OutputAssetName = $"{name}-output",
                JobName = $"{name}-job"
            };

        }

        /// <summary>
        /// Create the ServiceClientCredentials object based on the credentials
        /// supplied in local configuration file.
        /// </summary>
        /// <param name="config">The param is of type ConfigWrapper. This class reads values from local configuration file.</param>
        /// <returns></returns>
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
        /// <returns></returns>
        // <CreateMediaServicesClient>
        private async Task<IAzureMediaServicesClient> CreateMediaServicesClientAsync(AzureMediaServiceConfig config)
        {
            var credentials = await GetCredentialsAsync(config);

            return new AzureMediaServicesClient(config.ArmEndpoint, credentials)
            {
                SubscriptionId = config.SubscriptionId,
            };
        }
        // </CreateMediaServicesClient>

        /// <summary>
        /// Creates the input asset asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="amsConfig">The ams configuration.</param>
        /// <param name="assetIdentity">The asset identity.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        private async Task<Asset> CreateInputAssetAsync(
            IAzureMediaServicesClient client,
            AzureMediaServiceConfig amsConfig,
            AzureVideoAssetIdentity assetIdentity)
        {

            Asset asset = await client.Assets.CreateOrUpdateAsync(amsConfig.ResourceGroup, amsConfig.AccountName, assetIdentity.InputAssetName, new Asset());

            var response = await client.Assets.ListContainerSasAsync(
                amsConfig.ResourceGroup,
                amsConfig.AccountName,
                assetIdentity.InputAssetName,
                permissions: AssetContainerPermission.ReadWrite,
                expiryTime: DateTime.UtcNow.AddHours(4).ToUniversalTime());

            var sasUri = new Uri(response.AssetContainerSasUrls.First());

            BlobContainerClient container = new BlobContainerClient(sasUri);
            BlobClient blob = container.GetBlobClient(Path.GetFileName(assetIdentity.FileName));

            this.UploadVideo(blob, assetIdentity);

            return asset;
        }

        /// <summary>
        /// Uploads the video.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <param name="assetIdentity">The asset identity.</param>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        private void UploadVideo(BlobClient blob, AzureVideoAssetIdentity assetIdentity)
        {
            var directory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var videoLocation = Path.Combine(directory, VideoStoragePath, assetIdentity.FileName);
            if (!File.Exists(videoLocation)) throw new FileNotFoundException();

            Progress<long> progress = new Progress<long>();
            progress.ProgressChanged += Progress_ProgressChanged;

            var file = File.ReadAllBytes(videoLocation);
            var stream = new MemoryStream(file);

            Debug.WriteLine("starting uploading... " + assetIdentity.Name);

            blob.Upload(stream, null, null, null, progress, null);

            Debug.WriteLine("finished uploading " + assetIdentity.Name);
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

        /// <summary>
        /// Creates an output asset. The output from the encoding Job must be written to an Asset.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="outputAssetName">The output asset name.</param>
        /// <returns></returns>
        // <CreateOutputAsset>
        private async Task<Asset> CreateOutputAssetAsync(
            IAzureMediaServicesClient client,
            AzureMediaServiceConfig amsConfig,
            string outputAssetName)
        {
            // Check if an Asset already exists
            Asset inputAsset = await client.Assets.GetAsync(amsConfig.ResourceGroup, amsConfig.AccountName, outputAssetName);
            Asset outputAsset = new Asset();

            if (inputAsset != null)
            {
                throw new ArgumentException("the output asset name already exists", outputAssetName);
            }

            return await client.Assets.CreateOrUpdateAsync(amsConfig.ResourceGroup, amsConfig.AccountName, outputAssetName, outputAsset);
        }
        // </CreateOutputAsset>

        /// <summary>
        /// If the specified transform exists, get that transform.
        /// If the it does not exist, creates a new transform with the specified output. 
        /// In this case, the output is set to encode a video using one of the built-in encoding presets.
        /// </summary>
        /// <param name="azureMediaServiceClient">The Media Services client.</param>
        /// <returns></returns>
        // <EnsureTransformExists>
        private async Task<Transform> GetOrCreateTransformAsync(
            IAzureMediaServicesClient azureMediaServiceClient,
            AzureMediaServiceConfig azureMediaServiceConfig)
        {
            // Does a Transform already exist with the desired name? Assume that an existing Transform with the desired name
            // also uses the same recipe or Preset for processing content.
            Transform transform = await azureMediaServiceClient.Transforms.
                GetAsync(azureMediaServiceConfig.ResourceGroup, azureMediaServiceConfig.AccountName, AdaptiveStreamingTransformName);

            if (transform == null)
            {
                throw new ArgumentException("could not find the specific transformer ", AdaptiveStreamingTransformName);
            }

            return transform;
        }
        // </EnsureTransformExists>

        /// <summary>
        /// Submits a request to Media Services to apply the specified Transform to a given input video.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        private async Task<Job> SubmitJobAsync(
            IAzureMediaServicesClient client,
            AzureMediaServiceConfig amsConfig,
            AzureVideoAssetIdentity assetIdentity)
        {
            // Use the name of the created input asset to create the job input.
            JobInput jobInput = new JobInputAsset(assetName: assetIdentity.InputAssetName);

            JobOutput[] jobOutputs =
            {
                 new JobOutputAsset(assetIdentity.OutputAssetName),
            };


            Job job = await client.Jobs.CreateAsync(
                amsConfig.ResourceGroup,
                amsConfig.AccountName,
                AdaptiveStreamingTransformName,
                assetIdentity.JobName,
                new Job
                {
                    Input = jobInput,
                    Outputs = jobOutputs,
                });

            return job;
        }
        // </SubmitJob>

        /// <summary>
        /// Polls Media Services for the status of the Job.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="jobName">The name of the job you submitted.</param>
        /// <returns></returns>
        // <WaitForJobToFinish>
        private async Task<Job> WaitForJobToFinishAsync(
            IAzureMediaServicesClient client,
            AzureMediaServiceConfig amsConfig,
            string jobName)
        {
            const int SleepIntervalMs = 20 * 1000;

            Job job;
            do
            {
                job = await client.Jobs.GetAsync(amsConfig.ResourceGroup, amsConfig.AccountName, AdaptiveStreamingTransformName, jobName);

                Debug.WriteLine($"Job is '{job.State}'.");
                for (int i = 0; i < job.Outputs.Count; i++)
                {
                    JobOutput output = job.Outputs[i];
                    Debug.Write($"\tJobOutput[{i}] is '{output.State}'.");
                    if (output.State == JobState.Processing)
                    {
                        Debug.Write($"  Progress (%): '{output.Progress}'.");
                    }

                    Debug.WriteLine("");
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
        /// <returns></returns>
        private async Task<StreamingLocator> CreateStreamingLocatorAsync(
            IAzureMediaServicesClient client,
            AzureMediaServiceConfig amsConfig,
            AzureVideoAssetIdentity azureVideoAsset)
        {
            StreamingLocator locator = await client.StreamingLocators.CreateAsync(
                amsConfig.ResourceGroup,
                amsConfig.AccountName,
                azureVideoAsset.LocatorName,
                new StreamingLocator
                {
                    AssetName = azureVideoAsset.OutputAssetName,
                    StreamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly
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
        /// <returns></returns>
        // <GetStreamingURLs>
        private async Task<string> GetStreamingUrlsAsync(
            IAzureMediaServicesClient client,
            string resourceGroupName,
            string accountName,
            String locatorName)
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

                    Path = path.Paths[0]
                };
                streamingUrls.Add(uriBuilder.ToString());
            }

            return streamingUrls[2];
        }

        /// <summary>
        /// Deletes the jobs, assets and potentially the content key policy that were created.
        /// Generally, you should clean up everything except objects 
        /// that you are planning to reuse (typically, you will reuse Transforms, and you will persist output assets and StreamingLocators).
        /// </summary>
        /// <param name="client"></param>
        /// <param name="jobName"></param>
        /// <param name="assetNames"></param>
        /// <param name="contentKeyPolicyName"></param>
        /// <returns></returns>
        // <CleanUp>
        private async Task CleanUpAsync(
           IAzureMediaServicesClient client,
           AzureMediaServiceConfig config,
           string jobName,
           List<string> assetNames = null,
           string contentKeyPolicyName = null
           )
        {
            await client.Jobs.DeleteAsync(config.ResourceGroup, config.AccountName, AdaptiveStreamingTransformName, jobName);

        }
    }
}
