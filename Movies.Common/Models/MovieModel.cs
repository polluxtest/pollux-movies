﻿using System;
using System.Collections.Generic;

namespace Movies.Common.Models
{
    public class MovieModel : MovieCommonModel
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
        /// Gets or sets the description eng.
        /// </summary>
        /// <value>
        /// The description en.
        /// </value>
        public string DescriptionEs { get; set; }


        /// <summary>
        /// Gets or sets the movie genre generic.
        /// </summary>
        /// <value>
        /// The movie genre.
        /// </value>
        public string Genre { get; set; }

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
        /// Gets or sets the URL cover image.
        /// </summary>
        /// <value>
        /// The URL cover image.
        /// </value>
        public string UrlCoverImage { get; set; }

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
        public string DirectorName { get; set; }

        /// <summary>
        /// Gets or sets the likes.
        /// </summary>
        /// <value>
        /// The likes.
        /// </value>
        public int Likes { get; set; }

        /// <summary>
        /// Gets or sets the subtitles.
        /// </summary>
        /// <value>
        /// The subtitles.
        /// </value>
        public List<string> Subtitles { get; set; }

        /// <summary>
        /// Gets or sets the imbd.
        /// </summary>
        /// <value>
        /// The imbd.
        /// </value>
        public string Imbd { get; set; }

        /// <summary>
        /// Gets or sets the genres.
        /// </summary>
        /// <value>
        /// The genres.
        /// </value>
        public List<string> CategoryGenres { get; set; }
    }

}

