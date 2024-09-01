/// <summary>
/// Validates parameters for LikesController but also work for List controller
/// </summary>

namespace Pollux.Movies.Validators
{
    using System;

    using FluentValidation;

    using global::Movies.Common.Models.Requests;

    using MovieUserRequest = global::Movies.Common.Models.Requests.MovieUserRequest;

    /// <summary>
    /// Vlidates MovieUserRequest
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Movies.Common.Models.Requests.MovieUserRequest&gt;" />
    public class MoviePostLikesValidator : AbstractValidator<MovieUserRequest>
    {
        public MoviePostLikesValidator()
        {
            this.RuleFor(p => p.MovieId).NotEqual(Guid.Empty).NotNull().NotEmpty().WithMessage("Invalid movie id");
            this.RuleFor(p => p.UserId).NotNull().NotEmpty().WithMessage("Invalid User id");
        }
    }

    /// <summary>
    /// Validates UserIdRequest
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Movies.Common.Models.Requests.UserIdRequest&gt;" />
    public class MovieGetLikesValidator : AbstractValidator<UserIdRequest>
    {
        public MovieGetLikesValidator()
        {
            this.RuleFor(p => p.UserId).NotNull().NotEmpty().WithMessage("Invalid User id");
        }
    }
}