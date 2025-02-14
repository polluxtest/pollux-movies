﻿using System;

namespace Movies.Common.Models.Requests
{
    public class SearchMoviesRequest
    {
        /// <summary>
        /// Gets or sets the search.
        /// </summary>
        /// <value>
        /// The search.
        /// </value>
        public string Search { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the sort by.
        /// </summary>
        /// <value>
        /// The sort by.
        /// </value>
        public string SortBy { get; set; }
    }
}