using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Movies.Domain.Entities
{
    public class MovieFeatured
    {
        public int Id { get; set; }

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

        /// <summary>
        /// Gets or sets the URL portrait image.
        /// </summary>
        /// <value>
        /// The URL portrait image.
        /// </value>
        public string UrlPortraitImage { get; set; }
    }
}
