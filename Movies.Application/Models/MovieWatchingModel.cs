namespace Movies.Application.Models
{
    using System;

    public class MovieWatchingModel
    {
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
        /// The movie identifier.
        /// </value>
        public MovieModel Movie { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string UserId { get; set; }

        /// <summary>Gets or sets the elapsed time.</summary>
        /// <value>The elapsed time.</value>
        public int ElapsedTime { get; set; }

        /// <summary>Gets or sets the duration.</summary>
        /// <value>The duration.</value>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the remaining time.
        /// </summary>
        /// <value>
        /// The remaining time.
        /// </value>
        public int RemainingTime { get; set; }
    }
}
