using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Common.Constants.Strings;

namespace Pollux.Movies.Controllers
{
    /// <summary>
    /// Defines the Base Controllers attributes for the core domain <see cref="BaseController" />.
    /// </summary>
    [Route(ApiRoutesConstants.DefaultRoute)]
    [Authorize(AuthenticationSchemes = AuthConstants.TokenAuthenticationDefaultScheme)]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected bool IsUserIdValid(string userId)
        {
            return string.IsNullOrEmpty(userId);
        }
    }
}
