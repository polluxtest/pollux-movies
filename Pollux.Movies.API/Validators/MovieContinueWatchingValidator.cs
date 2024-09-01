namespace Pollux.Movies.Validators
{
    using FluentValidation;

    using global::Movies.Common.Models.Requests;

    /// <summary>
    /// Validates MovieWatchingRequestValidator
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Movies.Common.Models.Requests.MovieContinueWatchingRequest&gt;" />
    public class MovieWatchingRequestValidator : AbstractValidator<MovieContinueWatchingRequest>
    {
        public MovieWatchingRequestValidator()
        {
            this.RuleFor(p => p.MovieId).NotNull().NotEmpty().WithMessage("Invalid movie id");
            this.RuleFor(p => p.UserId).NotNull().NotEmpty().WithMessage("Invalid User id");
        }
    }

    /// <summary>
    /// validates MovieWatchingSaveRequest
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Movies.Common.Models.Requests.MovieWatchingSaveRequest&gt;" />
    public class SaveMovieContinueWatchingValitor : AbstractValidator<MovieWatchingSaveRequest>
    {
        public SaveMovieContinueWatchingValitor()
        {
            this.RuleFor(p => p.MovieId).NotNull().NotEmpty().WithMessage("Invalid movie id");
            this.RuleFor(p => p.UserId).NotNull().NotEmpty().WithMessage("Invalid User id");
        }
  }
}
