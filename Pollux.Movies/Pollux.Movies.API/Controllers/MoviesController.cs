using System;
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
        [Route(ApiRoutesConstants.ByLanguage)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByLanguage(string sortBy = null)
        {
            var moviesByLanguage = await this.moviesService.GetByLanguage(sortBy);

            return this.Ok(moviesByLanguage);
        }

        /// <summary>
        /// Gets the by director.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>Movies By Director.</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.ByDirector)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByDirector(string sortBy = null)
        {
            var moviesByDirector = await this.moviesService.GetByDirector(sortBy);

            return this.Ok(moviesByDirector);
        }

        /// <summary>
        /// Gets the by director.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>Movies By Director.</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.ByGenre)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByGenre(string sortBy = null)
        {
            var moviesByGenre = await this.moviesService.GetByGenreAsync(sortBy);

            return this.Ok(moviesByGenre);
        }


        /// <summary>
        /// Searches the specified search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns>Movie List.</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.Search)]
        public async Task<ActionResult<List<MovieModel>>> Search(string search)
        {
            if (string.IsNullOrEmpty(search)) return this.BadRequest("invalid search text");

            var searchMovies = await this.moviesService.Search(search);

            return this.Ok(searchMovies);
        }

        /// <summary>
        /// Gets the movie.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>MovieInfoModel.</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.Movie)]
        public async Task<ActionResult<MovieInfoModel>> GetMovie([FromRoute] Guid id, [FromQuery] string userId)
        {
            try
            {
                var movie = await this.moviesService.GetAsync(id, userId);
                return this.Ok(movie);
            }
            catch (ArgumentException)
            {
                return this.NotFound();
            }
        }

        /// <summary>
        /// Recommended the by pollux.
        /// </summary>
        /// <returns>List<MoviesByCategoryModel/></returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 8200, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.RecommendedByPollux)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> RecommendedByPollux()
        {
            var recommendedMovies = await this.moviesService.GetRecommendedByPollux();

            return this.Ok(recommendedMovies);
        }

        /// <summary>
        /// Recommended the by Users.
        /// </summary>
        /// <returns>List<MoviesByCategoryModel/></returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 8200, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.RecommendedByUsers)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> RecommendedByUsers()
        {
            var recommendedMovies = await this.moviesService.GetRecommendedByUsers();

            return this.Ok(recommendedMovies);
        }

        /// <summary>
        /// Gets the movies names.
        /// </summary>
        /// <returns>List<string/></returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 8200, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.GetMovieNames)]
        public async Task<ActionResult<List<string>>> GetMoviesNames()
        {
            var movieNames = await this.moviesService.GetMoviesNames();

            return movieNames;
        }

    }
}
