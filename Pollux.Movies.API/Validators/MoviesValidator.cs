using System;
using System.Collections.Generic;
using FluentValidation;
using Movies.Application.Models.Requests;
using Movies.Common.Constants.Strings;

namespace Pollux.Movies.Validators
{
    public class MoviesValidator : AbstractValidator<GetMoviesByCategoryRequest>
    {
        public MoviesValidator()
        {
            this.RuleFor(p => p.UserId).NotNull().NotEmpty().WithMessage("Invalid user id");

            this.RuleFor(p => p.SortBy).Must(p =>
            {
                if (string.IsNullOrEmpty(p)) return true;

                var sortTypes = new List<string>()
                {
                    SortByConstants.Imbd,
                    SortByConstants.AlphaAscending,
                    SortByConstants.AlphaDescending,
                    SortByConstants.Featured,
                    SortByConstants.Magic,
                };

                return sortTypes.Contains(p);
            });
        }
    }

    public class MoviesSearchValidator : AbstractValidator<SearchMoviesRequest>
    {
        public MoviesSearchValidator()
        {
            this.RuleFor(p => p.UserId).NotEqual(Guid.Empty).NotNull().NotEmpty().WithMessage("Invalid user id");
            this.RuleFor(p => p.Search).NotNull().NotEmpty().WithMessage("Invalid search id");
        }
    }

    public class MovieGetValidator : AbstractValidator<GetMovieRequest>
    {
        public MovieGetValidator()
        {
            this.RuleFor(p => p.Id).NotEqual(Guid.Empty).NotNull().NotEmpty().WithMessage("Invalid movie id");
            this.RuleFor(p => p.UserId).NotEqual(Guid.Empty).NotNull().NotEmpty().WithMessage("Invalid User id");
        }
    }

    public class MovieSearchValidator : AbstractValidator<SearchMoviesRequest>
    {
        public MovieSearchValidator()
        {
            this.RuleFor(p => p.Search).NotNull().NotEmpty().WithMessage("Invalid search id");
            this.RuleFor(p => p.UserId).NotEqual(Guid.Empty).NotNull().NotEmpty().WithMessage("Invalid User id");
        }
    }
}
