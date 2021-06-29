using System;
using FluentValidation;
using Movies.Application.Models;

namespace Pollux.Movies.Validators
{
    public class AddMovieToMyListValidator : AbstractValidator<AddRemoveUserMovieModel>
    {
        public AddMovieToMyListValidator()
        {
            this.RuleFor(p => p.MovieId).NotNull().NotEqual(0).WithMessage("Invalid movie id");
            this.RuleFor(p => p.UserId).NotNull().NotEmpty().NotEqual(Guid.Empty).WithMessage("Invalid User id");
        }
    }
}