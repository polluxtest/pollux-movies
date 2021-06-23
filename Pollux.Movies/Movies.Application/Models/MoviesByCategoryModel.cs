using System.Collections.Generic;

namespace Movies.Application.Models
{
    public class MoviesByCategoryModel
    {
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the movies.
        /// </summary>
        /// <value>
        /// The movies.
        /// </value>
        public List<MovieModel> Movies { get; set; }
    }
}
