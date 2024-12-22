namespace Movies.Common.Models.Requests
{
    public class GetMoviesByDirectorRequest
    {
        /// <summary>
        /// Gets or sets the director identifier.
        /// </summary>
        /// <value>
        /// The director identifier.
        /// </value>
        public int DirectorId { get; set; }

        /// <summary>
        /// Gets or sets the sort by.
        /// </summary>
        /// <value>
        /// The sort by.
        /// </value>
        public string SortBy { get; set; }
    }
}
