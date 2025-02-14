﻿namespace Pollux.Movies.Controllers
{
    using global::Movies.Common.Constants.Strings;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the Base Controllers attributes for the core domain <see cref="BaseController" />.
    /// </summary>
    [Route(ApiRoutesConstants.DefaultRoute)]
    [Authorize(AuthenticationSchemes = AuthConstants.TokenAuthenticationDefaultScheme)]
    [ApiController]
    public class BaseController : ControllerBase
    {
    }
}