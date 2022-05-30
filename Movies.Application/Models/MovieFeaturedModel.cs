using System;
using System.Collections.Generic;
using System.Text;

namespace Movies.Application.Models
{
    public class MovieFeaturedModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid MovieId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the director.
        /// </summary>
        /// <value>
        /// The name of the director.
        /// </value>
        public string DirectorName { get; set; }

        /// <summary>
        /// Gets or sets the URL portrait image.
        /// </summary>
        /// <value>
        /// The URL portrait image.
        /// </value>
        public string UrlPortraitImage { get; set; }
    }
}
