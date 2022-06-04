namespace Movies.Application.ExtensionMethods
{
    using Movies.Common.Constants.Strings;
    using Movies.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class MoviesDbExtensionMethods
    {
        /// <summary>
        /// Sorts the custom by.
        /// </summary>
        /// <param name="movies">The movies.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>IEnumerable<Movie/></returns>
        public static List<Movie> SortCustomBy(this List<Movie> movies, string sortBy = null)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                var rng = new Random();
                var moviesRandomOrder = movies.OrderBy(a => rng.Next()).ToList();
                return moviesRandomOrder;
            }

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
