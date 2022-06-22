using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application;
using Movies.Application.Models;
using Movies.Common.Constants.Strings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pollux.Movies.Controllers
{
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
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [AllowAnonymous]
        [Route(ApiRoutesConstants.FeaturedMovies)]
        public async Task<ActionResult<List<MovieFeaturedModel>>> GetFeaturedMovies()
        {
            var featuredMovies = await this.moviesService.GetAll();

            return this.Ok(featuredMovies);
        }
    }
}
