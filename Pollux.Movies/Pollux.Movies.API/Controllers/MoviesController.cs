using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application;
using Movies.Application.Models;
using Movies.Common.Constants.Strings;

namespace Pollux.Movies.Controllers
{
    public class MoviesController : BaseController
    {
        private readonly IMoviesService moviesService;

        public MoviesController(IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        /// <summary>
        /// Gets the by language.
        /// </summary>
        /// <param name="director">The director.</param>
        /// <returns>List Movie By Language Model.</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesRouteConstants.ByLanguage)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByLanguage()
        {
            var moviesByLanguage = await this.moviesService.GetByLanguage();

            return this.Ok(moviesByLanguage);
        }

        /// <summary>
        /// Gets the by language.
        /// </summary>
        /// <param name="director">The director.</param>
        /// <returns>List Movie By Language Model.</returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        [Route(ApiRoutesRouteConstants.ByDirector)]
        public async Task<ActionResult<List<MoviesByCategoryModel>>> GetByDireactor()
        {
            var moviesByDireactor = await this.moviesService.GetByDirector();

            return this.Ok(moviesByDireactor);
        }
    }
}
