using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Movies.Application;
using global::Movies.Application.Models;
using global::Movies.Common.Constants.Strings;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Models.Requests;
using Movies.Common.Constants;

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
        /// <param name="request">The Request UserId and sort.</param>
        /// <returns>List<MoviesByCategoryModel></returns>
        [HttpGet]
        [ResponseCache(
            Duration = ResponseCachaTimes.ThirtyMinutes,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [Route(ApiRoutesConstants.ByLanguage)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByLanguage(
            [FromQuery] GetMoviesByCategoryRequest request)
        {
            var moviesByLanguage =
                await this.moviesService.GetByLanguageAsync(request.UserId.ToString(), request.SortBy);
            return this.Ok(moviesByLanguage);
        }

        /// <summary>
        /// Gets tmovies grouped by director.
        /// </summary>
        /// <param name="request">The Request UserId and sort.</param>
        /// <returns>List<MoviesByCategoryModel></returns>
        [ResponseCache(
            Duration = ResponseCachaTimes.ThirtyMinutes,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
        [Route(ApiRoutesConstants.ByDirector)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByDirector(
            [FromQuery] GetMoviesByCategoryRequest request)
        {
            var moviesByDirector =
                await this.moviesService.GetByDirectorAsync(request.UserId.ToString(), request.SortBy);
            return this.Ok(moviesByDirector);
        }

        /// <summary>
        /// Gets movies grouped by genre
        /// </summary>
        /// <param name="request">The Request UserId and sort.</param>
        /// <returns>List<MoviesByCategoryModel></returns>
        [ResponseCache(
            Duration = ResponseCachaTimes.ThirtyMinutes,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
        [Route(ApiRoutesConstants.ByGenre)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByGenre(
            [FromQuery] GetMoviesByCategoryRequest request)
        {
            var moviesByGenre = await this.moviesService.GetByGenreAsync(request.UserId.ToString(), request.SortBy);

            return this.Ok(moviesByGenre);
        }

        /// <summary>
        /// Searches the specified search.
        /// </summary>
        /// <param name="request">The search request.</param>
        /// <returns>List<MovieModel></returns>
        [ResponseCache(
            Duration = ResponseCachaTimes.OneMinute,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
        [Route(ApiRoutesConstants.Search)]
        public async Task<ActionResult<List<MovieModel>>> Search(SearchMoviesRequest request)
        {
            var searchMovies = await this.moviesService.Search(request.Search, request.UserId.ToString());
            return this.Ok(searchMovies);
        }

        /// <summary>
        /// Gets the movie.
        /// </summary>
        /// <param name="request">The request to get the movie.</param>
        /// <returns>MovieInfoModel.</returns>
        [ResponseCache(
            Duration = ResponseCachaTimes.TenMinutes,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
        [Route(ApiRoutesConstants.Movie)]
        public async Task<ActionResult<MovieModel>> GetMovie(GetMovieRequest request)
        {
            try
            {
                var movie = await this.moviesService.GetAsync(request.Id, request.UserId.ToString());
                return this.Ok(movie);
            }
            catch (ArgumentException)
            {
                return this.NotFound();
            }
        }

        /// <summary>
        /// Recommendeds the by pollux.
        /// </summary>
        /// <param name="request">The user identifier.</param>
        /// <returns><List<MoviesByCategoryModel></returns>
        [ResponseCache(
            Duration = ResponseCachaTimes.ThirtyMinutes,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
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
            Duration = ResponseCachaTimes.ThirtyMinutes,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
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
            Duration = ResponseCachaTimes.OneHour,
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