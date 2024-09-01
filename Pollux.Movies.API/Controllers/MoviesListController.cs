namespace Pollux.Movies.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Movies.Application.Services;
    using global::Movies.Common.Constants.Strings;
    using global::Movies.Common.Models;
    using global::Movies.Common.Models.Requests;

    using Microsoft.AspNetCore.Mvc;

    using MovieUserRequest = global::Movies.Common.Models.Requests.MovieUserRequest;

    public class MoviesListController : BaseController
    {
        private readonly IMoviesListService userMoviesService;

        public MoviesListController(IMoviesListService userMoviesService)
        {
            this.userMoviesService = userMoviesService;
        }

        /// <summary>
        /// Gets the specified user identifier.
        /// </summary>
        /// <param name="request">The user identifier.</param>
        /// <returns>List of My movie ids.</returns>
        [HttpGet]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> Get([FromQuery] UserIdRequest request)
        {
            var userMovieList = await this.userMoviesService.GetMovieMyList(request.UserId.ToString());

            return this.Ok(userMovieList);
        }

        /// <summary>
        /// Gets the specified user identifier.
        /// </summary>
        /// <param name="request">The user identifier.</param>
        /// <returns>List of My movies.</returns>
        [HttpGet]
        [Route(ApiRoutesConstants.GetMyListIds)]
        public async Task<ActionResult<List<Guid>>> GetMyListIds([FromQuery] UserIdRequest request)
        {
            var userMovieList = await this.userMoviesService.GetMoviesIdsByUser(request.UserId);

            return this.Ok(userMovieList);
        }

        /// <summary>
        /// Posts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Movie id added.</returns>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] MovieUserRequestGuid request)
        {
            await this.userMoviesService.AddRemoveAsync(request);

            return this.Ok();
        }
    }
}