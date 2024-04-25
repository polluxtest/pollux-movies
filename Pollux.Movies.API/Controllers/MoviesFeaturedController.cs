using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application;
using Movies.Application.Models;
using Movies.Common.Constants;

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
        [ResponseCache(
            Duration = ResponseCachaTimes.ThirtyMinutes,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<MovieFeaturedModel>>> Get()
        {
            var featuredMovies = await this.moviesService.GetAll();
            return this.Ok(featuredMovies);
        }
    }
}
