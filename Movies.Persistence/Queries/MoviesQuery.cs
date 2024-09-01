using System.Collections.Generic;
using Movies.Common.Models;
using Movies.Domain.Entities;

namespace Movies.Persistence.Queries
{
    public class MoviesQuery : MovieCommonModel
    {
        /// <summary>
        /// Gets or sets the movie.
        /// </summary>
        /// <value>
        /// The movie.
        /// </value>
        public Movie Movie { get; set; }

        /// <summary>
        /// Gets or sets the genre.
        /// </summary>
        /// <value>
        /// The genre.
        /// </value>
        public string Genre { get; set; }

        /// <summary>
        /// Gets or sets the category genre.
        /// </summary>
        /// <value>
        /// The category genre.
        /// </value>
        public IEnumerable<string> CategoryGenres { get; set; }

        /// <summary>
        /// Gets or sets the director.
        /// </summary>
        /// <value>
        /// The director.
        /// </value>
        public Director Director { get; set; }
    }
}