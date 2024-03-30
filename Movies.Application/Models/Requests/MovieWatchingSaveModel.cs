namespace Movies.Application.Models.Requests
{
    public class MovieWatchingSaveModel : MovieUserRequest
    {
        /// <summary>Gets or sets the elapsed time.</summary>
        /// <value>The elapsed time.</value>
        public int ElapsedTime { get; set; }

        /// <summary>Gets or sets the duration.</summary>
        /// <value>The duration.</value>
        public int Duration { get; set; }
    }
}
