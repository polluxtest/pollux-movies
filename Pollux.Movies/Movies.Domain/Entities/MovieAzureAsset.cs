namespace Movies.Domain.Entities
{
    public class MovieAzureAsset
    {
        /// <summary>
        /// Gets or sets the movie identifier.
        /// </summary>
        /// <value>
        /// The movie identifier.
        /// </value>
        public int MovieId { get; set; }

        /// <summary>
        /// Gets or sets the movie.
        /// </summary>
        /// <value>
        /// The movie.
        /// </value>
        public Movie Movie { get; set; }

        /// <summary>
        /// Gets or sets the name of the asset input.
        /// </summary>
        /// <value>
        /// The name of the asset input.
        /// </value>
        public string AssetInputName { get; set; }

        /// <summary>
        /// Gets or sets the asset output.
        /// </summary>
        /// <value>
        /// The asset output.
        /// </value>
        public string AssetOutput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [proccesed by azure job].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [proccesed by azure job]; otherwise, <c>false</c>.
        /// </value>
        public bool ProccesedByAzureJob { get; set; }
    }
}
