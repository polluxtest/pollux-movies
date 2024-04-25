using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Models.Requests;

namespace Pollux.Movies.Validators
{
    public class MovieWartchingRequestValidator : AbstractValidator<MovieContinueWatchingRequest>
    {
        public MovieWartchingRequestValidator()
        {
            this.RuleFor(p => p.MovieId).NotNull().NotEmpty().WithMessage("Invalid movie id");
            this.RuleFor(p => p.UserId).NotNull().NotEmpty().WithMessage("Invalid User id");
        }
    }

    public class SaveMovieContinueWatchingValitor : AbstractValidator<MovieWatchingSaveModel>
    {
        public SaveMovieContinueWatchingValitor()
        {
            this.RuleFor(p => p.MovieId).NotNull().NotEmpty().WithMessage("Invalid movie id");
            this.RuleFor(p => p.UserId).NotNull().NotEmpty().WithMessage("Invalid User id");
        }
    }
}
