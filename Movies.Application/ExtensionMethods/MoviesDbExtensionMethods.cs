using Movies.Common.Constants.Strings;
using Movies.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Movies.Common.Models;

namespace Movies.Application.ExtensionMethods
{
    public static class MoviesDbExtensionMethods
    {
        /// <summary>
        /// Sorts the custom by.
        /// </summary>
        /// <param name="movies">The movies.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>List<MovieModel></returns>
        public static List<MovieModel> SortCustomBy(this List<MovieModel> movies, string sortBy = null)
        {
            switch (sortBy)
            {
                case SortByConstants.Magic:
                    movies = MagicSort(movies);
                    break;
                case SortByConstants.AlphaAscending:
                    movies = movies.OrderBy(p => p.Name).ToList();
                    break;
                case SortByConstants.AlphaDescending:
                    movies = movies.OrderByDescending(p => p.Name).ToList();
                    break;
                case SortByConstants.Imbd:
                    movies = movies.OrderByDescending(p => p.Imbd).ToList();
                    break;
                case SortByConstants.Featured:
                    movies = movies.OrderByDescending(p => p.Likes).ToList();
                    break;
                default:
                    movies = MagicSort(movies);
                    break;
            }

            return movies;
        }

        /// <summary>Magics the sort.</summary>
        /// <param name="movies">The movies.</param>
        /// <returns>List<Movie></Movie></returns>
        private static List<MovieModel> MagicSort(List<MovieModel> movies)
        {
            var rng = new Random();
            var moviesRandomOrder = movies.OrderBy(a => rng.Next()).ToList();
            return moviesRandomOrder;
        }
    }
}
