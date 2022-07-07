namespace Pollux.Movies.Controllers
{
    using global::Movies.Application;
    using global::Movies.Application.Models;
    using global::Movies.Common.Constants.Strings;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MoviesWatchingController : BaseController
    {
        private readonly IMoviesWatchingService moviesWatchingService;

        public MoviesWatchingController(IMoviesWatchingService moviesWatchingService)
        {
            this.moviesWatchingService = moviesWatchingService;
        }

        /// <summary>Posts the specified movie watching model.</summary>
        /// <param name="movieWatchingModel">The movie watching model.</param>
        /// <returns>Ok.</returns>
        [HttpPost]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Post([FromBody] MovieWatchingModel movieWatchingModel)
        {
            await this.moviesWatchingService.Save(movieWatchingModel);
            return this.NoContent();
        }

        /// <summary>Gets the specified user identifier.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>MovieWatchingModel.</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<MovieWatchingModel>> Get([FromQuery] string userId, [FromQuery] Guid movieId)
        {
            var movieWatchingModel = await this.moviesWatchingService.GetAsync(userId, movieId);
            return this.Ok(movieWatchingModel);
        }

        /// <summary>Gets all continue watching.</summary>
        /// <param name="userId">The User Id</param>
        /// <returns>List<MovieWatchingModel/></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [Route(ApiRoutesConstants.GetAllContinueWatching)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetAllContinueWatching([FromQuery] string userId)
        {
            var moviesWatching = await this.moviesWatchingService.GetAllAsync(userId);

            return this.Ok(moviesWatching);
        }
    }
}
