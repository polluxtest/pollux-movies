using System;

namespace Movies.Domain.Entities
{
    public class UserMovies
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the movie identifier.
        /// </summary>
        /// <value>
        /// The movie identifier.
        /// </value>
        public Guid MovieId { get; set; }

        /// <summary>
        /// Gets or sets the movie.
        /// </summary>
        /// <value>
        /// The movie.
        /// </value>
        public Movie Movie { get; set; }

    }
}
