using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadFilesService;

namespace Pollux.Movies.Controllers
{
    public class ReadCopyController : BaseController
    {
        private readonly IFileReader fileReader;

        public ReadCopyController(IFileReader fileReader)
        {
            this.fileReader = fileReader;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/CopyMovies")]
        public async Task<IActionResult> CopyMovies()
        {
            await this.fileReader.ReadVideosFromDirectory();

            return this.Ok();
        }


        [HttpGet]
        [Route("/CopyImages")]
        [AllowAnonymous]
        public async Task<IActionResult> CopyImages()
        {
            await this.fileReader.ReadImagesFromDirectory();

            return this.Ok();
        }


        [HttpGet]
        [Route("/CopySubtitles")]
        [AllowAnonymous]
        public async Task<IActionResult> CopySubtitles()
        {
            await this.fileReader.ReadSubtitlesFromDirectory();

            return this.Ok();
        }

    }
}