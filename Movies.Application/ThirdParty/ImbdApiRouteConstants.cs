using System;
using System.Collections.Generic;
using System.Text;

namespace Movies.Application.ThirdParty
{
    public static class ImbdApiRouteConstants
    {
        /// <summary>
        /// The get imbd movie code
        /// </summary>
        public const string GetImbdMovieByCode = "https://imdb8.p.rapidapi.com/title/get-ratings?tconst=";

        /// <summary>
        /// The get rating by imbd code
        /// </summary>
        public const string GetImbdMovieByName = "https://imdb8.p.rapidapi.com/title/find?q=";
    }
}
