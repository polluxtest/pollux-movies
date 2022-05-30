namespace Movies.Application.Models
{
    public class MovieInfoModel : MovieModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is liked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is liked; otherwise, <c>false</c>.
        /// </value>
        public bool IsLiked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in list.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is in list; otherwise, <c>false</c>.
        /// </value>
        public bool IsInList { get; set; }
    }
}
