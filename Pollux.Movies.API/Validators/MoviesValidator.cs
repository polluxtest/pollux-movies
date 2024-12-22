namespace Pollux.Movies.Validators
{
    using System;
    using System.Collections.Generic;

    using FluentValidation;

    using global::Movies.Common.Constants.Strings;
    using global::Movies.Common.Models.Requests;

    /// <summary>
    /// Validates the GetMoviesByCategoryRequest requet
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Movies.Common.Models.Requests.GetMoviesByCategoryRequest&gt;" />
    public class MoviesValidator : AbstractValidator<GetMoviesByCategoryRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoviesValidator"/> class.
        /// </summary>
        public MoviesValidator()
        {
            this.RuleFor(p => p.UserId).NotNull().NotEmpty().WithMessage("Invalid user id");

            this.RuleFor(p => p.SortBy).Must(
                p =>
                    {
                        if (string.IsNullOrEmpty(p))
                        {
                            return true;
                        }

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

    /// <summary>
    /// Validates thte SearchMoviesRequest
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Movies.Common.Models.Requests.SearchMoviesRequest&gt;" />
    public class MoviesSearchValidator : AbstractValidator<SearchMoviesRequest>
    {
        public MoviesSearchValidator()
        {
            this.RuleFor(p => p.UserId).NotNull().NotEmpty().WithMessage("Invalid user id");
            this.RuleFor(p => p.Search).NotNull().NotEmpty().WithMessage("search is empry id");
        }
    }

    /// <summary>
    /// Validates GetMovieRequest
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Movies.Common.Models.Requests.GetMovieRequest&gt;" />
    public class MovieGetValidator : AbstractValidator<GetMovieRequest>
    {
        public MovieGetValidator()
        {
            this.RuleFor(p => p.Id).NotEqual(Guid.Empty).NotNull().NotEmpty().WithMessage("Invalid movie id");
            this.RuleFor(p => p.UserId).NotNull().NotEmpty().WithMessage("Invalid User id");
        }
    }

    /// <summary>
    /// Validates SearchByGenreRequest 
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Movies.Common.Models.Requests.SearchByGenreRequest&gt;" />
    public class SearchByGenreValidator : AbstractValidator<SearchByGenreRequest>
    {
        public SearchByGenreValidator()
        {
            this.RuleFor(p => p.SortBy).NotNull().NotEmpty().WithMessage("Invalid sort");
            this.RuleFor(p => p.Genre).NotNull().NotEmpty().WithMessage("Invalid genre");
            this.RuleFor(p => p.UserId).NotNull().NotEmpty().WithMessage("Invalid User id");
        }
    }

    /// <summary>
    /// Validates Request for Movies By Director Id
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Movies.Common.Models.Requests.GetMoviesByDirectorRequest&gt;" />
    public class GetMoviesByDirectorIdValidator : AbstractValidator<GetMoviesByDirectorRequest>
    {
        public GetMoviesByDirectorIdValidator()
        {
            this.RuleFor(p => p.DirectorId).GreaterThan(0).WithMessage("Invalid Ditector Id");
            //this.RuleFor(p => p.SortBy).NotNull().NotEmpty().WithMessage("Invalid sort");
        }
    }
}
