using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.ThirdParty;

namespace Pollux.Movies.Controllers
{
    public class GoogleTranslateController : BaseController
    {
        private readonly ITranslationService translationService;

        public GoogleTranslateController(ITranslationService translationService)
        {
            this.translationService = translationService;
        }

        /// <summary>
        /// Translates this instance.
        /// </summary>
        /// <returns>Task.</returns>
        [Route("translateDescription")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TranslateDescription()
        {
            await this.translationService.TranslateEnToEs();
            return this.Ok();
        }
    }
}
