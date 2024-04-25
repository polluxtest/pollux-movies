using System;
using Microsoft.AspNetCore.Mvc;

namespace Movies.Application.Models.Requests
{
    public class GetMoviesByCategoryRequest
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [FromQuery]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the sort by.
        /// </summary>
        /// <value>
        /// The sort by.
        /// </value>
        [FromQuery]
        public string SortBy { get; set; } = null;
    }
}
