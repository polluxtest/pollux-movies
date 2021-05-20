using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Common.Constants.Strings;

namespace Pollux.Movies.Controllers
{
    /// <summary>
    /// Defines the Base Controllers attributes for the core domain <see cref="BaseController" />.
    /// </summary>
    [Route(ApiConstants.DefaultRoute)]
    [Authorize]
    [ApiController]
    public class BaseController : ControllerBase
    {
    }
}
