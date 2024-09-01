using System.Collections.Generic;

namespace Movies.Common.Models
{
    public class MoviesByCategoryModel
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
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
