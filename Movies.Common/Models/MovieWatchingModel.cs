using System;

namespace Movies.Common.Models
{
    public class MovieWatchingModel : MovieCommonModel
    {
        /// <summary>
        /// Gets or sets the movie.
        /// </summary>
        /// <value>
        /// The movie identifier.
        /// </value>
        public MovieModel Movie { get; set; }
    }
}