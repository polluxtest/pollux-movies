using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application;
using Movies.Application.ThirdParty;
using ReadFilesService;

namespace Pollux.Movies.Controllers.Azure
{
    public class ReadCopyController : BaseController
    {
        private readonly IFileReader fileReader;
        private readonly IMoviesGenresService genresService;
        private readonly IMoviesServiceAzure moviesService;

        public ReadCopyController(
            IFileReader fileReader,
            IMoviesGenresService genresService,
            IMoviesServiceAzure moviesService)
        {
            this.fileReader = fileReader;
            this.genresService = genresService;
            this.moviesService = moviesService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/CopyMovies")]
        public async Task<IActionResult> CopyMovies()
        {
            await fileReader.ReadVideosFromDirectory();

            return Ok();
        }


        [HttpGet]
        [Route("/CopyImages")]
        [AllowAnonymous]
        public async Task<IActionResult> CopyImages()
        {
            await fileReader.ReadImagesFromDirectory();

            return Ok();
        }


        [HttpGet]
        [Route("/CopySubtitles")]
        [AllowAnonymous]
        public async Task<IActionResult> CopySubtitles()
        {
            await fileReader.ReadSubtitlesFromDirectory();

            return Ok();
        }

        [HttpGet]
        [Route("/CopyCoverImages")]
        [AllowAnonymous]
        public async Task<IActionResult> CopyCoverImages()
        {
            await fileReader.ReadCoverImagesFromDirectory();

            return Ok();
        }

        [HttpGet]
        [Route("/AddGenres")]
        [AllowAnonymous]
        public async Task<IActionResult> AddGenres()
        {
            var genresList = await fileReader.ReadGenresFromFile();
            genresList = genresList.OrderBy(p => p).ToList();
            await genresService.AddAsync(genresList);
            var movieGenres = await fileReader.ReadMovieGenresFromFile();
            await moviesService.AddGenresAsync(movieGenres);
            return Ok();
        }

    }
}