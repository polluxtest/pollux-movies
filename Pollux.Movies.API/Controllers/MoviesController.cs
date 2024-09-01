namespace Pollux.Movies.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Movies.Application.Services;
    using global::Movies.Common.Constants;
    using global::Movies.Common.Constants.Strings;
    using global::Movies.Common.Models;
    using global::Movies.Common.Models.Requests;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
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
        /// <param name="request">The Request UserId and sort.</param>
        /// <returns>List<MoviesByCategoryModel></returns>
        [HttpGet]
        [ResponseCache(
            Duration = ResponseCacheTimes.TwoHours,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [Route(ApiRoutesConstants.ByLanguage)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByLanguage(
            [FromQuery] GetMoviesByCategoryRequest request)
        {
            var moviesByLanguage =
                await this.moviesService.GetByLanguageAsync(request.UserId, request.SortBy);
            return this.Ok(moviesByLanguage);
        }

        /// <summary>
        /// Gets tmovies grouped by director.
        /// </summary>
        /// <param name="request">The Request UserId and sort.</param>
        /// <returns>List<MoviesByCategoryModel></returns>
        [ResponseCache(
            Duration = ResponseCacheTimes.TwoHours,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
        [Route(ApiRoutesConstants.ByDirector)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByDirector(
            [FromQuery] GetMoviesByCategoryRequest request)
        {
            var moviesByDirector =
                await this.moviesService.GetByDirectorAsync(request.UserId, request.SortBy);
            return this.Ok(moviesByDirector);
        }

        /// <summary>
        /// Gets movies grouped by genre
        /// </summary>
        /// <param name="request">The Request UserId and sort.</param>
        /// <returns>List<MoviesByCategoryModel></returns>
        [ResponseCache(
            Duration = ResponseCacheTimes.TwoHours,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
        [ProducesResponseType(200)]
        [Route(ApiRoutesConstants.ByCategoryGenre)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByCategoryGenre(
            [FromQuery] GetMoviesByCategoryRequest request)
        {
            var moviesByGenre = await this.moviesService.GetByCategoryGenresAsync(request.UserId, request.SortBy);

            return this.Ok(moviesByGenre);
        }

        /// <summary>
        /// Searches the specified search.
        /// </summary>
        /// <param name="request">The search request.</param>
        /// <returns>List<MovieModel></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [Route(ApiRoutesConstants.Search)]
        public async Task<ActionResult<List<MovieModel>>> Search([FromQuery] SearchMoviesRequest request)
        {
            var searchMovies = await this.moviesService.Search(request.Search, request.UserId, request.SortBy);
            return this.Ok(searchMovies);
        }

        /// <summary>
        /// Recommendeds the by pollux.
        /// </summary>
        /// <param name="request">The user identifier.</param>
        /// <returns><List<MoviesByCategoryModel></returns>
        [ResponseCache(
            Duration = ResponseCacheTimes.OneHour,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
        [ProducesResponseType(200)]
        [Route(ApiRoutesConstants.RecommendedByPollux)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> RecommendedByPollux(
            [FromQuery] UserIdRequest request)
        {
            var recommendedMovies = await this.moviesService.GetRecommendedByPollux(request.UserId.ToString());

            return this.Ok(recommendedMovies);
        }

        /// <summary>
        /// Recommended the by Users.
        /// </summary>
        /// <param name="request">The user identifier.</param>
        /// <returns>List<MoviesByCategoryModel/></returns>
        [ResponseCache(
            Duration = ResponseCacheTimes.ThirtyMinutes,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
        [ProducesResponseType(200)]
        [Route(ApiRoutesConstants.RecommendedByUsers)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> RecommendedByUsers(
            [FromQuery] UserIdRequest request)
        {
            var recommendedMovies = await this.moviesService.GetRecommendedByUsers(request.UserId.ToString());

            return this.Ok(recommendedMovies);
        }

        /// <summary>
        /// Gets the movie search options.
        /// </summary>
        /// <returns>List<string>></returns>
        [ResponseCache(
            Duration = ResponseCacheTimes.TwoHours,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
        [HttpGet]
        [Route(ApiRoutesConstants.GetMovieSearchOptions)]
        public async Task<ActionResult<List<string>>> GetMovieSearchOptions()
        {
            var movieSearchResultOptions = await this.moviesService.GetMovieSearchOptions();
            return movieSearchResultOptions;
        }
    }
}