using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using global::Movies.Application;
using global::Movies.Application.Models;
using global::Movies.Common.Constants.Strings;
using Microsoft.AspNetCore.Mvc;

namespace Pollux.Movies.Controllers
{
    public class MoviesController : BaseController
    {
        private readonly IMoviesService moviesService;

        public MoviesController(IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        [HttpGet]
        [Route(ApiRoutesConstants.ByLanguage)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByLanguage(string userId, string sortBy = null)
        {
            var moviesByLanguage = await this.moviesService.GetByLanguageAsync(userId, sortBy);
            return this.Ok(moviesByLanguage);
        }

        /// <summary>
        /// Gets tmovies grouped by director.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>List<MoviesByCategoryModel></returns>
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.ByDirector)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByDirector(string userId, string sortBy = null)
        {
            var moviesByDirector = await this.moviesService.GetByDirectorAsync(userId, sortBy);
            return this.Ok(moviesByDirector);
        }

        /// <summary>
        /// Gets movies grouped by genre
        /// </summary>
        /// <param name="userId">The User Id</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>Movies By Director.</returns>
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.ByGenre)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByGenre(string userId, string sortBy = null)
        {
            var moviesByGenre = await this.moviesService.GetByGenreAsync(userId, sortBy);

            return this.Ok(moviesByGenre);
        }

        /// <summary>
        /// Searches the specified search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user Id.</param>
        /// <returns>Movie List.</returns>
        //[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.Search)]
        public async Task<ActionResult<List<MovieModel>>> Search(string search, string userId)
        {
            var searchMovies = await this.moviesService.Search(search, userId);
            return this.Ok(searchMovies);
        }

        /// <summary>
        /// Gets the movie.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>MovieInfoModel.</returns>
        //[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.Movie)]
        public async Task<ActionResult<MovieModel>> GetMovie([FromRoute] Guid id, [FromQuery] string userId)
        {
            try
            {
                var watch = new Stopwatch();
                watch.Start();
                var movie = await this.moviesService.GetAsync(id, userId);
                Debug.WriteLine(TimeSpan.FromMilliseconds((double)watch.ElapsedMilliseconds).TotalSeconds);
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
        [ResponseCache(Duration = 8200, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.GetMovieSearchOptions)]
        public async Task<ActionResult<List<string>>> GetMovieSearchOptions()
        {
            var movieSearchResultOptions = await this.moviesService.GetMovieSearchOptions();
            return movieSearchResultOptions;
        }
    }
}