using System.Linq;
using System.Threading.Tasks;
using AzureUploaderTransformerVideos.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application;
using Movies.Common.Constants.Strings;
using ReadFilesService;

namespace Pollux.Movies.Controllers
{
    public class ReadCopyController : BaseController
    {
        private readonly IFileReader fileReader;
        private readonly IGenresService genresService;
        private readonly IMoviesService moviesService;

        public ReadCopyController(
            IFileReader fileReader,
            IGenresService genresService,
            IMoviesService moviesService)
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

        [HttpGet]
        [Route("/CopyCoverImages")]
        [AllowAnonymous]
        public async Task<IActionResult> CopyCoverImages()
        {
            await this.fileReader.ReadCoverImagesFromDirectory();

            return this.Ok();
        }

        [HttpGet]
        [Route("/AddGenres")]
        [AllowAnonymous]
        public async Task<IActionResult> AddGenres()
        {
            var genresList = await this.fileReader.ReadGenresFromFile();
            genresList = genresList.OrderBy(p => p).ToList();
            await this.genresService.AddAsync(genresList);
            var movieGenres = await this.fileReader.ReadMovieGenresFromFile();
            await this.moviesService.AddGenresAsync(movieGenres);
            return this.Ok();
        }

    }
}