using System;
using FluentValidation;
using Movies.Application.Models;

namespace Pollux.Movies.Validators
{
    public class AddMovieToMyListValidator : AbstractValidator<AddRemoveUserMovieModel>
    {
        public AddMovieToMyListValidator()
        {
            this.RuleFor(p => p.MovieId).NotNull().NotEmpty().WithMessage("Invalid movie id");
            this.RuleFor(p => p.UserId).NotNull().NotEmpty().WithMessage("Invalid User id");
        }
    }
}