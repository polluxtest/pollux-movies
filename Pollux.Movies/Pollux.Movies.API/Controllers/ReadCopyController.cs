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
        public async Task<IActionResult> Get()
        {
            await this.fileReader.ReadVideosFromDirectory();

            return this.Ok();
        }


        [HttpGet]
        [Route("/CopyMovies")]
        [AllowAnonymous]
        public async Task<IActionResult> CopyMovies()
        {
            await this.fileReader.ReadImagesFromDirectory();

            return this.Ok();
        }
    }
}
