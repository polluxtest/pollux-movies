using Movies.Domain.Entities;

namespace Movies.Application.Models
{
    public class MovieGenreModel
    {
        /// <summary>
        /// Gets or sets the movie.
        /// </summary>
        /// <value>
        /// The movie.
        /// </value>
        public MovieModel Movie { get; set; }

        /// <summary>
        /// Gets or sets the genre.
        /// </summary>
        /// <value>
        /// The genre.
        /// </value>
        public Genre Genre { get; set; }
    }
}
