namespace Pollux.Movies.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MovieUploaderTransformerService;
    using Pollux.API.Controllers;

    public class AzureMediaServicesController : BaseController
    {
        private readonly AzureMediaService azureMediaService;

        public AzureMediaServicesController(AzureMediaService azureMediaService)
        {
            this.azureMediaService = azureMediaService;
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
    }
}
