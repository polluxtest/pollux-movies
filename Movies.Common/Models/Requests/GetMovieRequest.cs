using System;
using Microsoft.AspNetCore.Mvc;

namespace Movies.Common.Models.Requests
{
    public class GetMovieRequest
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [FromRoute] public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [FromQuery]
        public string UserId { get; set; }
    }
}
