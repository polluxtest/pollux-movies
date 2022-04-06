using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application;
using Movies.Application.Models;
using Movies.Common.Constants.Strings;

namespace Pollux.Movies.Controllers
{
    public class MovieLikesController : BaseController
    {
        private readonly IUserLikesService userLikesService;

        public MovieLikesController(IUserLikesService userLikesService)
        {
            this.userLikesService = userLikesService;
        }

        /// <summary>
        /// Likes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task.</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route(ApiRoutesConstants.Like)]
        public async Task<ActionResult> Like([FromBody] AddRemoveUserMovieModel request)
        {
            await this.userLikesService.AddRemoveUserLike(request);

            return this.Ok();
        }

        /// <summary>
        /// Gets the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List<Guid></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route(ApiRoutesConstants.MyLikes)]
        public async Task<ActionResult<List<System.Guid>>> Get([FromQuery] string userId)
        {
            if (base.IsUserIdValid(userId)) return this.BadRequest("Invalid User Id");

            var moviesLikes = await this.userLikesService.GetLikesMoviesIds(userId);

            return this.Ok(moviesLikes);
        }
    }
}
