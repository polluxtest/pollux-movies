using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movies.Common.Constants.Strings;
using Movies.Domain.Entities;

namespace Movies.Application.ExtensionMethods
{
    public static class MoviesDbExtensionMethods
    {
        /// <summary>
        /// Sorts the custom by.
        /// </summary>
        /// <param name="movies">The movies.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>IEnumerable<Movie>.</returns>
        public static List<Movie> SortCustomBy(this List<Movie> movies, string sortBy = null)
        {
            if (string.IsNullOrEmpty(sortBy)) return movies;

            switch (sortBy)
            {
                case SortByConstants.AlphaAscending: movies = movies.OrderBy(p => p.Name).ToList(); break;
                case SortByConstants.AlphaDescending: movies = movies.OrderByDescending(p => p.Name).ToList(); break;
                case SortByConstants.Imbd: movies = movies.OrderByDescending(p => p.Imbd).ToList(); break;
                case SortByConstants.Featured: movies = movies.OrderByDescending(p => p.Likes).ToList(); break;
            }

            return movies;
        }
    }
}
