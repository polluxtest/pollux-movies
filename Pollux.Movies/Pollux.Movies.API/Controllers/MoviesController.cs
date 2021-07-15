using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application;
using Movies.Application.Models;
using Movies.Common.Constants.Strings;

namespace Pollux.Movies.Controllers
{
    public class MoviesController : BaseController
    {
        private readonly IMoviesService moviesService;

        public MoviesController(IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        /// <summary>
        /// Gets the by language.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>List Movie By Language Model.</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesRouteConstants.ByLanguage)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByLanguage(string sortBy = null)
        {
            var moviesByLanguage = await this.moviesService.GetByLanguage(sortBy);

            return this.Ok(moviesByLanguage);
        }


        /// <summary>
        /// Searches the specified search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns>Movie List.</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesRouteConstants.Search)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> Search(string search)
        {
            if (string.IsNullOrEmpty(search)) return this.BadRequest("invalid search text");

            var searchMovies = await this.moviesService.Search(search);

            return this.Ok(searchMovies);
        }

        /// <summary>
        /// Gets the by direactor.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>Movies By Direactor.</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesRouteConstants.ByDirector)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByDirector(string sortBy = null)
        {
            var moviesByDirector = await this.moviesService.GetByDirector(sortBy);

            return this.Ok(moviesByDirector);
        }
    }
}
