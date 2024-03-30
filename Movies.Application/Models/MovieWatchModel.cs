namespace Movies.Application.Models
{
    public class MovieWatchModel
    {
        /// <summary>Gets or sets the elapsed time.</summary>
        /// <value>The elapsed time.</value>
        public double ElapsedTime { get; set; }

        /// <summary>Gets or sets the duration.</summary>
        /// <value>The duration.</value>
        public double Duration { get; set; }

        /// <summary>
        /// Gets or sets the remaining time.
        /// </summary>
        /// <value>
        /// The remaining time.
        /// </value>
        public int RemainingTime { get; set; }
    }
}
