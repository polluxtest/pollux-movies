using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Movies.Application;
using Movies.Application.Models;
using Movies.Application.Models.Requests;
using Movies.Common.Constants;
using Movies.Common.Constants.Strings;
using Movies.Persistence.Repositories;

namespace Pollux.Movies.Controllers
{
    public class MoviesGenresController : BaseController
    {
        private readonly IMoviesByGenresService moviesGenresService;

        public MoviesGenresController(IMoviesByGenresService moviesByGenresService)
        {
            this.moviesGenresService = moviesByGenresService;
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>List<string>></returns>
        [HttpGet]
        [ResponseCache(
            Duration = ResponseCachaTimes.ThirtyMinutes,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        public async Task<ActionResult<List<string>>> Get()
        {
            var genres = await this.moviesGenresService.GetGenresAsync();
            return this.Ok(genres);
        }

        /// <summary>
        /// Searches the by genre.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>List<MovieModel></returns>
        [HttpGet]
        [Route(ApiRoutesConstants.GetSearchByGenre)]
        [ResponseCache(
            Duration = ResponseCachaTimes.FiveMinutes,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        public async Task<ActionResult<List<MovieModel>>> SearchByGenre([FromQuery] SearchByGenreRequest request)
        {
            var genres =
                await this.moviesGenresService.GetSearchByGenreAsync(request.UserId, request.Genre, request.SortBy);
            return this.Ok(genres);
        }
    }
}
