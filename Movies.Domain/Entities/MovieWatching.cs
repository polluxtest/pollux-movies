namespace Movies.Domain.Entities
{
    using System;

    public class MovieWatching
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string UserId { get; set; }

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

        /// <summary>Gets or sets the elapsed time.</summary>
        /// <value>The elapsed time.</value>
        public decimal ElapsedTime { get; set; }

        /// <summary>Gets or sets the duration.</summary>
        /// <value>The duration.</value>
        public decimal Duration { get; set; }
    }
}
