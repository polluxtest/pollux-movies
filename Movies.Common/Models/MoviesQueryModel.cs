
namespace Movies.Common.Models
{
    public class MoviesQueryModel : MovieCommonModel
    {
        /// <summary>
        /// Gets or sets the movie.
        /// </summary>
        /// <value>
        /// The movie.
        /// </value>
        public MovieModel Movie { get; set; }


        /// <summary>
        /// Gets or sets the genres.
        /// </summary>
        /// <value>
        /// The genres.
        /// </value>
        public string[] CategoryGenres { get; set; }
    }
}
