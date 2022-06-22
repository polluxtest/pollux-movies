namespace Movies.Application.Models
{
    public class MovieFeaturedModel
    {
        /// <summary>Gets or sets the movie.</summary>
        /// <value>The movie.</value>
        public MovieInfoModel Movie { get; set; }

        /// <summary>
        /// Gets or sets the URL portrait image.
        /// </summary>
        /// <value>
        /// The URL portrait image.
        /// </value>
        public string UrlPortraitImage { get; set; }
    }
}
