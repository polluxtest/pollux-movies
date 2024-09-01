namespace Pollux.Movies.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Movies.Application.Services;
    using global::Movies.Common.Models.Requests;

    using Microsoft.AspNetCore.Mvc;

    using MovieUserRequest = global::Movies.Common.Models.Requests.MovieUserRequest;

    public class MovieLikesController : BaseController
    {
        private readonly IMoviesLikesService userLikesService;

        public MovieLikesController(IMoviesLikesService userLikesService)
        {
            this.userLikesService = userLikesService;
        }

        /// <summary>
        /// Likes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task.200</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult> Post([FromBody] MovieUserRequestGuid request)
        {
            await this.userLikesService.AddRemoveUserLike(request);

            return this.Ok();
        }

        /// <summary>
        /// Gets the specified user identifier.
        /// </summary>
        /// <param name="request">The user identifier.</param>
        /// <returns>List<Guid></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<System.Guid>>> Get([FromQuery] UserIdRequest request)
        {
            var moviesLikes = await this.userLikesService.GetLikesMoviesIds(request.UserId);

            return this.Ok(moviesLikes);
        }
    }
}