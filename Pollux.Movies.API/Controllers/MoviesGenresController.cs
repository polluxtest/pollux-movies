namespace Pollux.Movies.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Movies.Application.Services;
    using global::Movies.Common.Constants;
    using global::Movies.Common.Constants.Strings;
    using global::Movies.Common.Models;
    using global::Movies.Common.Models.Requests;

    using Microsoft.AspNetCore.Mvc;

    public class MoviesGenresController : BaseController
    {
        private readonly IMoviesByGenresService moviesGenresService;
        private readonly IMoviesService moviesService;

        public MoviesGenresController(IMoviesByGenresService moviesByGenresService, IMoviesService moviesService)
        {
            this.moviesGenresService = moviesByGenresService;
            this.moviesService = moviesService;
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>List<string>></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ResponseCache(
            Duration = ResponseCacheTimes.TwoHours,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        public async Task<ActionResult<List<string>>> Get()
        {
            var genres = await this.moviesService.GetGenresAsync();
            return this.Ok(genres);
        }

        /// <summary>
        /// Searches the by genre.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>List<MovieModel></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [Route(ApiRoutesConstants.GetSearchByGenre)]
        [ResponseCache(
            Duration = ResponseCacheTimes.OneHour,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        public async Task<ActionResult<List<MovieModel>>> SearchByGenre([FromQuery] SearchByGenreRequest request)
        {
            var genres =
                await this.moviesGenresService.GetSearchByGenreAsync(
                    request.UserId,
                    request.Genre,
                    request.SortBy);
            return this.Ok(genres);
        }
    }
}