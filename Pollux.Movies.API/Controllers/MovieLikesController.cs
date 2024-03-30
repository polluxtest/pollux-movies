using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Movies.Application;
using Movies.Application.Models;
using Movies.Common.Constants.Strings;

namespace Pollux.Movies.Controllers
{
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
        /// <returns>Task.</returns>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AddRemoveUserMovieModel request)
        {
            await this.userLikesService.AddRemoveUserLike(request);

            return this.Ok();
        }

        /// <summary>
        /// Gets the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List<Guid></returns>
        [Route(ApiRoutesConstants.GetLikesIds)]
        [HttpGet]
        public async Task<ActionResult<List<System.Guid>>> GetLikesIds([FromQuery] string userId)
        {
            if (this.IsUserIdValid(userId)) return this.BadRequest("Invalid User Id");

            var moviesLikes = await this.userLikesService.GetLikesMoviesIds(userId);

            return this.Ok(moviesLikes);
        }
    }
}
