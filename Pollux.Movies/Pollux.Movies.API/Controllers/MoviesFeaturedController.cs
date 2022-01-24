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
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesConstants.FeaturedMovies)]
        public async Task<ActionResult<List<MovieFeaturedModel>>> GetFeaturedMovies()
        {
            var featuredMovies = await this.moviesService.GetAll();

            return this.Ok(featuredMovies);
        }
    }
}
