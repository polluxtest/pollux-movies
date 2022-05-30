using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.ThirdParty;
using Movies.Common.Constants.Strings;

namespace Pollux.Movies.Controllers
{
    public class ImbdController : BaseController
    {
        private readonly IImbdService imbdService;

        public ImbdController(IImbdService imbdService)
        {
            this.imbdService = imbdService;
        }

        [Route(ApiRoutesConstants.GetImbdMovieRanking)]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetImbdMovieRanking()
        {
            await this.imbdService.FindImbdMovie();
            return this.Ok();
        }
    }
}
