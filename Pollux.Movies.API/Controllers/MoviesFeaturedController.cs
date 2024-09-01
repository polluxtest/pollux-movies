namespace Pollux.Movies.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Movies.Application.Services;
    using global::Movies.Common.Constants;
    using global::Movies.Persistence.Queries;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class MoviesFeaturedController : BaseController
    {
        private readonly IMoviesFeaturedService moviesService;

        public MoviesFeaturedController(IMoviesFeaturedService moviesService)
        {
            this.moviesService = moviesService;
        }

        /// <summary>
        /// Gets the by language.
        /// </summary>
        /// <returns>List Movie By Language Model.</returns>
        [ResponseCache(
            Duration = ResponseCacheTimes.TwoHours,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<MovieFeaturedQuery>>> Get()
        {
            var featuredMovies = await this.moviesService.GetAll();
            return this.Ok(featuredMovies);
        }
    }
}