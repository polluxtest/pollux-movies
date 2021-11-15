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
        private readonly IUserMoviesService userMoviesService;

        public MoviesListController(IUserMoviesService userMoviesService)
        {
            this.userMoviesService = userMoviesService;
        }

        /// <summary>
        /// Gets the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List of My movie ids.</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.MyList)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetMyList([FromQuery] string userId)
        {
            var userMovieList = await this.userMoviesService.GetMovieMyList(userId);

            return this.Ok(userMovieList);
        }

        /// <summary>
        /// Gets the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List of My movies.</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.MyListIds)]
        public async Task<ActionResult<List<int>>> Get([FromQuery] string userId)
        {
            if (base.IsUserIdValid(userId)) return this.BadRequest("Invalid User Id");

            var userMovieList = await this.userMoviesService.GetMoviesIdsByUser(userId);

            return this.Ok(userMovieList);
        }


        /// <summary>
        /// Posts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Movie id added.</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route(ApiRoutesConstants.UpdateList)]
        public async Task<ActionResult<int>> Post([FromBody] AddRemoveUserMovieModel request)
        {
            await this.userMoviesService.AddRemoveAsync(request);

            return this.Ok();
        }

    }
}
