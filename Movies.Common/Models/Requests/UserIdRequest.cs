using System;
using Microsoft.AspNetCore.Mvc;

namespace Movies.Common.Models.Requests
{
    public class UserIdRequest
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [FromQuery]
        public Guid UserId { get; set; }
    }
}
