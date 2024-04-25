using System;
using FluentValidation;
using Movies.Application.Models.Requests;
using MovieUserRequest = Movies.Application.Models.MovieUserRequest;

/// <summary>
/// Validates parameters for LikesController but also work for List controller
/// </summary>
namespace Pollux.Movies.Validators
{
    public class MoviePostLikesValidator : AbstractValidator<MovieUserRequest>
    {
        public MoviePostLikesValidator()
        {
            this.RuleFor(p => p.MovieId).NotEqual(Guid.Empty).NotNull().NotEmpty().WithMessage("Invalid movie id");
            this.RuleFor(p => p.UserId).NotEqual(Guid.Empty).NotNull().NotEmpty().WithMessage("Invalid User id");
        }
    }

    public class MovieGetLikesValidator : AbstractValidator<UserIdRequest>
    {
        public MovieGetLikesValidator()
        {
            this.RuleFor(p => p.UserId).NotEqual(Guid.Empty).NotNull().NotEmpty().WithMessage("Invalid User id");
        }
    }
}