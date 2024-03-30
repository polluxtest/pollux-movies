using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application;
using Movies.Application.Models;
using Movies.Common.Constants.Strings;

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
        /// <param name="userId">The user identifier.</param>
        /// <returns>List of My movie ids.</returns>
        [HttpGet]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> Get([FromQuery] string userId)
        {
            var userMovieList = await this.userMoviesService.GetMovieMyList(userId);

            return this.Ok(userMovieList);
        }

        /// <summary>
        /// Gets the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List of My movies.</returns>
        [HttpGet]
        [Route(ApiRoutesConstants.GetMyListIds)]
        public async Task<ActionResult<List<Guid>>> GetMyListIds([FromQuery] string userId)
        {
            if (this.IsUserIdValid(userId)) return this.BadRequest("Invalid User Id");

            var userMovieList = await this.userMoviesService.GetMoviesIdsByUser(userId);

            return this.Ok(userMovieList);
        }

        /// <summary>
        /// Posts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Movie id added.</returns>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] AddRemoveUserMovieModel request)
        {
            await this.userMoviesService.AddRemoveAsync(request);

            return this.Ok();
        }

    }
}
