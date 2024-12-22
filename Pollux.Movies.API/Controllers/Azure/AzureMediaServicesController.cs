using System.Linq;
using System.Threading.Tasks;
using AzureUploaderTransformerVideos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Services;
using Movies.Persistence.Repositories;

namespace Pollux.Movies.Controllers.Azure
{
    public class AzureMediaServicesController : BaseController
    {
        private readonly AzureMediaService azureMediaService;
        private readonly IAzureBlobsService azureBlobsService;
        private readonly IMoviesService moviesService;

        public AzureMediaServicesController(AzureMediaService azureMediaService, IAzureBlobsService azureBlobsService, IMoviesService moviesService)
        {
            this.azureMediaService = azureMediaService;
            this.azureBlobsService = azureBlobsService;
            this.moviesService = moviesService;
        }

        /// <summary>
        /// Execute the job of azure media services from the project Uploader Transformer Service.
        /// </summary>
        /// <returns>200.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            await this.azureMediaService.RunAsync();
            return this.Ok();
        }

        /// <summary>
        /// Execute the job of azure media services from the project Uploader Transformer Service.
        /// </summary>
        /// <returns>200.</returns>
        [HttpGet]
        [Route("ChangeManifestToDashVideo")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeManifestToDashVideo()
        {
            await this.azureMediaService.ChangeURLVideoManifestToDashProtocol();
            return this.Ok();
        }

        [HttpGet]
        [Route("GetAllBlobsFromContainer")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBlobsFromContainer()
        {
            var list = await this.azureBlobsService.GetAllFileNamesByContainerName("polluximagescovercontainer");
            var movies = await this.moviesService.GetAllAsync();

            foreach (var item in movies)
            {
                try
                {
                    var blobName = list.SingleOrDefault(p => p.Contains(item.Name));
                    var updatedUrl = $"https://polluxcdn-apade6ahamhkgchz.z03.azurefd.net/polluximagescovercontainer/{blobName}";
                    item.UrlCoverImage = updatedUrl;
                    await this.moviesService.Update(item);
                }
                catch { }
            }

            return this.Ok();
        }
    }
}