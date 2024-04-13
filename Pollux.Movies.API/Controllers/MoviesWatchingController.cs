using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Movies.Application;
using global::Movies.Application.Models;
using global::Movies.Application.Models.Requests;
using global::Movies.Common.Constants.Strings;
using Microsoft.AspNetCore.Mvc;

namespace Pollux.Movies.Controllers
{
    public class MoviesWatchingController : BaseController
    {
        private readonly IMoviesWatchingService moviesWatchingService;
        private readonly IMoviesService moviesService;

        public MoviesWatchingController(IMoviesWatchingService moviesWatchingService, IMoviesService moviesService)
        {
            this.moviesWatchingService = moviesWatchingService;
            this.moviesService = moviesService;
        }

        /// <summary>Posts the specified movie watching model.</summary>
        /// <param name="movieWatchingModel">The movie watching model.</param>
        /// <returns>Ok.</returns>
        [HttpPost]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Post([FromBody] MovieWatchingSaveModel movieWatchingModel)
        {
            await this.moviesWatchingService.Save(movieWatchingModel);
            return this.NoContent();
        }

        /// <summary>
        /// Gets the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>MovieWatchingModel</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<MovieWatchingModel>> Get([FromQuery] MovieContinueWatchingRequest request)
        {
            try
            {
                var movieWatching = await this.moviesService.GetAsync(request.MovieId, request.UserId);
                return this.Ok(movieWatching);
            }
            catch (ArgumentException e)
            {
                return this.NotFound();
            }
        }

        /// <summary>Gets all continue watching.</summary>
        /// <param name="userId">The User Id</param>
        /// <returns>List<MovieWatchingModel/></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [Route(ApiRoutesConstants.GetAllContinueWatching)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetAllContinueWatching([FromQuery] string userId)
        {
            var moviesWatching = await this.moviesWatchingService.GetAllMoviesContinueWatchingAsync(userId);

            return this.Ok(moviesWatching);
        }
    }
}
