namespace Movies.Application.Models
{
    using System;

    public class MovieWatchingModel
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

        /// <summary>Gets or sets the elapsed time.</summary>
        /// <value>The elapsed time.</value>
        public decimal ElapsedTime { get; set; }

        /// <summary>Gets or sets the duration.</summary>
        /// <value>The duration.</value>
        public decimal Duration { get; set; }
    }
}
