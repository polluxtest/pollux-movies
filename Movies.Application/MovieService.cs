using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.ExtensionMethods;
using Movies.Application.Models;
using Movies.Common.Constants;
using Movies.Common.Constants.Strings;
using Movies.Common.ExtensionMethods;
using Movies.Domain.Entities;
using Movies.Persistence.QueryResults;
using Movies.Persistence.Repositories;
using Pitcher;

namespace Movies.Application
{
    public interface IMoviesService
    {
        Task<List<MoviesByCategoryModel>> GetByLanguageAsync(string userId, string sortBy = null);

        Task<List<MoviesByCategoryModel>> GetByDirectorAsync(string userId, string sortBy = null);

        Task<List<MoviesByCategoryModel>> GetByGenreAsync(string userId, string sortBy = null);

        Task Update(Movie movie);

        Task AddAsync(Movie movie);

        Task<List<MovieModel>> Search(string search, string userId);

        Task<List<MoviesByCategoryModel>> GetRecommendedByUsers(string userId);

        Task<List<MoviesByCategoryModel>> GetRecommendedByPollux(string userId);

        Task<List<string>> GetMovieSearchOptions();

        Task<MovieWatchingModel> GetAsync(Guid movieId, string userId);
    }

    public class MoviesService : IMoviesService
    {
        private readonly IMoviesRepository moviesRepository;
        private readonly IMoviesByGenresService movieGenresService;
        private readonly IMapper mapper;

        public MoviesService(
            IMoviesRepository moviesRepository,
            IMoviesByGenresService movieGenreService,
            IMapper mapper)
        {
            this.moviesRepository = moviesRepository;
            this.movieGenresService = movieGenreService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets the by language asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>List<MoviesByCategoryModel>></returns>
        public async Task<List<MoviesByCategoryModel>> GetByLanguageAsync(string userId, string sortBy = null)
        {
            var moviesDb = await this.moviesRepository.GetAllAsync(userId);

            var moviesGroupedByLanguage = moviesDb
                .GroupBy(p => p.Movie.Language)
                .Select(p =>
                {
                    var moviesByCategory = new MoviesByCategoryModel()
                    {
                        Title = $"{p.Key} {TitleConstants.Language}",
                        Movies = this.mapper.Map<List<MoviesQueryResult>, List<MovieModel>>(p.ToList())
                            .SortCustomBy(sortBy),
                    };

                    return moviesByCategory;
                }).OrderByDescending(y => y.Movies.Count());

            var movies = moviesGroupedByLanguage.ToList();

            return movies;
        }

        /// <summary>
        /// Gets the by director asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>Task<List<MoviesByCategoryModel>></returns>
        public async Task<List<MoviesByCategoryModel>> GetByDirectorAsync(string userId, string sortBy = null)
        {
            var moviesDb = await this.moviesRepository.GetAllAsync(userId);

            var moviesGroupedByDirector = moviesDb
                .GroupBy(p => p.Director.Name)
                .Select(p =>
                {
                    var moviesByCategory = new MoviesByCategoryModel()
                    {
                        Title = p.Key,
                        Movies = this.mapper.Map<List<MoviesQueryResult>, List<MovieModel>>(p.ToList())
                            .SortCustomBy(sortBy),
                    };

                    return moviesByCategory;
                }).OrderByDescending(y => y.Movies.Count()); // todo is this neccessary 

            var movies = moviesGroupedByDirector.ToList();

            return movies;
        }

        /// <summary>
        /// Gets the by genre asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>Task<List<MoviesByCategoryModel>></returns>
        public async Task<List<MoviesByCategoryModel>> GetByGenreAsync(string userId, string sortBy = null)
        {
            var movies = await this.movieGenresService.GetAllByGenreAsync(userId, sortBy);
            return movies;
        }

        /// <summary>
        /// Updates the specified movie.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns>Task.</returns>
        public async Task Update(Movie movie)
        {
            this.moviesRepository.Update(movie);
            await this.moviesRepository.SaveAsync();
        }

        /// <summary>
        /// Adds the specified movie.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns>Task.</returns>
        public async Task AddAsync(Movie movie)
        {
            await this.moviesRepository.AddAsync(movie);
            await this.moviesRepository.SaveAsync();
        }

        /// <summary>
        /// Searches the specified search takes 15 movies to avoid returning too many.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The User Id.</param>
        /// <returns>List<MovieModel></returns>
        public async Task<List<MovieModel>> Search(string search, string userId)
        {
            if (string.IsNullOrEmpty(search))
            {
                search = search.RandomLetter(97, 123);
            }

            var moviesDbSearch = await this.moviesRepository.Search(search, userId);
            var moviesResult = this.mapper.Map<List<MoviesQueryResult>, List<MovieModel>>(moviesDbSearch);

            return moviesResult.Take(MoviesSearchContants.MaxResultSearch).ToList();
        }

        /// <summary>
        /// Gets the recommended by users.
        /// </summary>
        /// <param name="userId">The User Id.</param>
        /// <returns>List<MoviesByCategoryModel/>.</returns>
        public async Task<List<MoviesByCategoryModel>> GetRecommendedByPollux(string userId)
        {
            var moviesDb = await this.moviesRepository.GetRecommendedByPolluxAsync(userId);
            var movies = this.mapper.Map<List<MoviesQueryResult>, List<MovieModel>>(moviesDb);

            return new List<MoviesByCategoryModel>() { new MoviesByCategoryModel() { Movies = movies, Title = TitleConstants.RecommendedByPollux } };
        }

        /// <summary>
        /// Gets the recommended by users.
        /// </summary>
        /// <param name="userId">The User Id.</param>
        /// <returns>List<MoviesByCategoryModel/>.</returns>
        public async Task<List<MoviesByCategoryModel>> GetRecommendedByUsers(string userId)
        {
            var moviesDb = await this.moviesRepository.GetRecommendedAsync(userId);

            var movies = this.mapper.Map<List<MoviesQueryResult>, List<MovieModel>>(moviesDb);

            return new List<MoviesByCategoryModel>() { new MoviesByCategoryModel() { Movies = movies, Title = TitleConstants.RecommendedByUsers } };

        }

        /// <summary>
        /// Gets the movie search options.
        /// </summary>
        /// <returns>List<string></returns>
        public async Task<List<string>> GetMovieSearchOptions()
        {
            var resultOptions = new List<string>();
            var moviesList = await this.moviesRepository.GetAllAsync();
            var directorsList = moviesList.Select(p => p.Director.Name).Distinct().ToList();
            var movieNamesList = moviesList.Select(p => p.Name).ToList();

            resultOptions.AddRange(movieNamesList);
            resultOptions.AddRange(directorsList);

            return resultOptions;
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>MovieWatchingModel</returns>
        public async Task<MovieWatchingModel> GetAsync(Guid movieId, string userId)
        {
            var movieDb = await this.moviesRepository.GetAsync(movieId, userId);

            Throw.When(movieDb == null, new ArgumentException($"movie not found id {movieId}"));

            var genres = await this.movieGenresService.GetAllByMovieIdAsync(movieId);
            var movieWatching = this.mapper.Map<MoviesQueryResult, MovieWatchingModel>(movieDb);

            movieWatching.Movie.Genres = genres;

            return movieWatching;
        }
    }
}
