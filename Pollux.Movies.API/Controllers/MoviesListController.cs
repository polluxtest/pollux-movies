using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Movies.Application;
using Movies.Application.Models;
using Movies.Application.Models.Requests;
using Movies.Common.Constants.Strings;
using MovieUserRequest = Movies.Application.Models.MovieUserRequest;

namespace Pollux.Movies.Controllers
{
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
        public async Task<ActionResult<List<MoviesByCategoryModel>>> Get(UserIdRequest request)
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
            var userMovieList = await this.userMoviesService.GetMoviesIdsByUser(request.UserId.ToString());

            return this.Ok(userMovieList);
        }

        /// <summary>
        /// Posts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Movie id added.</returns>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] MovieUserRequest request)
        {
            await this.userMoviesService.AddRemoveAsync(request);

            return this.Ok();
        }

    }
}
