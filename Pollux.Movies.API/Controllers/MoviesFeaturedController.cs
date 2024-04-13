using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application;
using Movies.Application.Models;

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
        public async Task<ActionResult<List<MovieFeaturedModel>>> Get()
        {
            var stop = new Stopwatch();
            stop.Start();
            var featuredMovies = await this.moviesService.GetAll();
            stop.Stop();
            Debug.WriteLine(stop.ElapsedMilliseconds);
            return this.Ok(featuredMovies);
        }
    }
}
