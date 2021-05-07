using Microsoft.AspNetCore.Mvc;
using Pollux.API.Controllers;

namespace Pollux.Movies.Controllers
{
    public class Movies : BaseController
    {
        // GET
        public IActionResult Index()
        {
            return this.Content("working");
        }
    }
}