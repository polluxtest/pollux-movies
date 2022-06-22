namespace Movies.Domain.Entities
{
    using System;

    /// <summary>
    /// Movie Entity.
    /// </summary>
    public class Movie
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the description english.
        /// </summary>
        /// <value>
        /// The description en.
        /// </value>
        public string DescriptionEs { get; set; }


        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        public string Year { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string UrlVideo { get; set; }

        /// <summary>
        /// Gets or sets the URL image.
        /// </summary>
        /// <value>
        /// The URL image.
        /// </value>
        public string UrlImage { get; set; }

        /// <summary>
        /// Gets or sets the Cover URL image.
        /// </summary>
        /// <value>
        /// The URL image.
        /// </value>
        public string UrlCoverImage { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [processed by azure job].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [processed by azure job]; otherwise, <c>false</c>.
        /// </value>
        public bool ProcessedByAzureJob { get; set; }

        /// <summary>
        /// Gets or sets the director identifier.
        /// </summary>
        /// <value>
        /// The director identifier.
        /// </value>
        public int DirectorId { get; set; }

        /// <summary>
        /// Gets or sets the director.
        /// </summary>
        /// <value>
        /// The director.
        /// </value>
        public Director Director { get; set; }

        /// <summary>
        /// Gets or sets the likes.
        /// </summary>
        /// <value>
        /// The likes.
        /// </value>
        public int Likes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Movie"/> is recommended.
        /// </summary>
        /// <value>
        ///   <c>true</c> if recommended; otherwise, <c>false</c>.
        /// </value>
        public bool Recommended { get; set; }

        /// <summary>
        /// Gets or sets the subtitles.
        /// </summary>
        /// <value>
        /// The subtitles.
        /// </value>
        public string Subtitles { get; set; }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the imbd ranking.
        /// </summary>
        /// <value>
        /// The imbd.
        /// </value>
        public string Imbd { get; set; }
    }
}