using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.ExtensionMethods;
using Movies.Application.Models;
using Movies.Application.Models.Requests;
using Movies.Common.Constants;
using Movies.Common.Constants.Strings;
using Movies.Common.ExtensionMethods;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;
using Pitcher;

namespace Movies.Application
{
    public interface IMoviesService
    {
        Task<List<Movie>> GetAll(bool processedByAzureJob = false);

        Task<List<Movie>> GetAllMoviesImagesFilter(bool processedByAzureJob = false);

        Task<List<Movie>> GetAllMoviesCoverImagesFilter(bool processedByAzureJob = false);

        Task<List<MoviesByCategoryModel>> GetByLanguageAsync(string userId, string sortBy = null);

        Task<List<MoviesByCategoryModel>> GetByDirectorAsync(string userId, string sortBy = null);

        Task<List<MoviesByCategoryModel>> GetByGenreAsync(string userId, string sortBy = null);

        Task UpdateMovie(Movie movie);

        Task AddAsync(Movie movie);

        Task<List<MovieModel>> Search(string search, string userId);

        Task<List<MoviesByCategoryModel>> GetRecommendedByUsers();

        Task<List<MoviesByCategoryModel>> GetRecommendedByPollux();

        Task AddGenresAsync(Dictionary<string, List<string>> movieGenres);

        Task<List<string>> GetMovieSearchOptions();

        Task<MovieWatchingModel> GetWatchingAsync(Guid movieId, string userId);

        Task<MovieWatchingModel> GetAsync(Guid movieId, string userId);
    }

    public class MoviesService : IMoviesService
    {
        private readonly IMoviesRepository moviesRepository;
        private readonly IMoviesGenresService moviesGenresService;
        private readonly IMoviesByGenresService movieGenresService;
        private readonly IMoviesWatchingService moviesWatchingService;
        private readonly IMapper mapper;

        public MoviesService(
            IMoviesRepository moviesRepository,
            IMoviesListService moviesListService,
            IMoviesLikesService moviesLikesService,
            IMoviesGenresService moviesGenresService,
            IMoviesByGenresService movieGenreService,
            IMoviesWatchingService moviesWatchingService,
            IMapper mapper)
        {
            this.moviesRepository = moviesRepository;
            this.moviesGenresService = moviesGenresService;
            this.movieGenresService = movieGenreService;
            this.moviesWatchingService = moviesWatchingService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets the by language.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>MovieModel List by Language.</returns>
        public async Task<List<MoviesByCategoryModel>> GetByLanguageAsync(string userId, string sortBy = null)
        {
            var moviesDb = await this.moviesRepository.GetAllAsync();
            moviesDb = moviesDb.SortCustomBy(sortBy);
            var moviesWatchingDb = await this.moviesWatchingService.GetAllAsync(userId);
            var moviesModels = this.SetContinueWatching(moviesDb, moviesWatchingDb);

            var moviesGroupedByLanguage = moviesModels
                .GroupBy(p => p.Language)
                .Select(p =>
                {
                    var moviesByCategory = new MoviesByCategoryModel()
                    {
                        Title = $"{p.Key} {TitleConstants.Language}",
                        Movies = p.ToList(),
                    };

                    return moviesByCategory;
                }).OrderByDescending(y => y.Movies.Count());

            var movies = moviesGroupedByLanguage.ToList();

            return movies;
        }

        /// <summary>
        /// Gets the by director.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>MovieModel List by Director.</returns>
        public async Task<List<MoviesByCategoryModel>> GetByDirectorAsync(string userId, string sortBy = null)
        {
            var moviesDb = await this.moviesRepository.GetAllAsync();
            moviesDb = moviesDb.SortCustomBy(sortBy);
            var moviesWatchingDb = await this.moviesWatchingService.GetAllAsync(userId);
            var moviesModels = this.SetContinueWatching(moviesDb, moviesWatchingDb);

            var moviesGroupedByDirector = moviesModels
                .GroupBy(p => p.DirectorName)
                .Select(p =>
                {
                    var moviesByCategory = new MoviesByCategoryModel()
                    {
                        Title = p.Key,
                        Movies = p.ToList(),
                    };

                    return moviesByCategory;
                }).OrderByDescending(y => y.Movies.Count());

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
        public async Task UpdateMovie(Movie movie)
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

            var moviesDbSearch = await this.moviesRepository.Search(search);
            var moviesWatchingDb =
                await this.moviesWatchingService.GetAllAsync(userId); // todo move this by controller execution 
            var searchResult = this.SetContinueWatching(moviesDbSearch, moviesWatchingDb);

            return searchResult.Take(MoviesSearchContants.MaxResultSearch).ToList();
        }

        /// <summary>
        /// Gets the recommended by users.
        /// </summary>
        /// <returns>List<MoviesByCategoryModel/>.</returns>
        public async Task<List<MoviesByCategoryModel>> GetRecommendedByPollux()
        {
            var moviesDb = await this.moviesRepository.GetRecommendedByPolluxAsync();

            var movies = this.mapper.Map<List<Movie>, List<MovieModel>>(moviesDb);

            return new List<MoviesByCategoryModel>() { new MoviesByCategoryModel() { Movies = movies, Title = TitleConstants.RecommendedByPollux } };
        }

        /// <summary>
        /// Gets the recommended by users.
        /// </summary>
        /// <returns>List<MoviesByCategoryModel/>.</returns>
        public async Task<List<MoviesByCategoryModel>> GetRecommendedByUsers()
        {
            var moviesDb = await this.moviesRepository.GetRecommendedAsync();

            var movies = this.mapper.Map<List<Movie>, List<MovieModel>>(moviesDb);

            return new List<MoviesByCategoryModel>() { new MoviesByCategoryModel() { Movies = movies, Title = TitleConstants.RecommendedByUsers } };

        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>MovieInfoModel.</returns>
        public async Task<MovieWatchingModel> GetAsync(Guid movieId, string userId)
        {
            // todo use mapper 
            var movieDb = await this.moviesRepository.GetAsync(movieId);
            var genres = await this.movieGenresService.GetAllByMovieIdAsync(movieId);
            var movieContinueWatching = await this.moviesWatchingService
                .GetAsync(new MovieContinueWatchingRequest() { UserId = userId, MovieId = movieId });

            Throw.When(movieDb == null, new ArgumentException($"movie not found id {movieId}"));

            var movieWatchingModel = this.mapper.Map<MovieWatching, MovieWatchingModel>(movieContinueWatching);
            movieWatchingModel.Movie.Genres = genres;

            return movieWatchingModel;
        }


        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>MovieInfoModel.</returns>
        public async Task<MovieWatchingModel> GetWatchingAsync(Guid movieId, string userId)
        {
            // todo use automapper here
            var genres = await this.movieGenresService.GetAllByMovieIdAsync(movieId);
            var movieContinueWatching = await this.moviesWatchingService
                .GetAsync(new MovieContinueWatchingRequest() { UserId = userId, MovieId = movieId });

            Throw.When(movieContinueWatching == null, new ArgumentException($"movie not found id {movieId}"));

            var movieWatchingModel = this.mapper.Map<MovieWatching, MovieWatchingModel>(movieContinueWatching);
            movieWatchingModel.Movie.Genres = genres;
            return movieWatchingModel;
        }

        /// <summary>
        /// Adds the genres asynchronous.
        /// </summary>
        /// <param name="movieGenres">The movie genres.</param>
        /// <exception cref="System.ArgumentException">Genre not found</exception>
        /// <returns>Task</returns>
        public async Task AddGenresAsync(Dictionary<string, List<string>> movieGenres)
        {
            var genresDb = await this.moviesGenresService.GetAllAsync();

            foreach (var (movieName, genres) in movieGenres)
            {
                var movieDb = await this.GetByNameAsync(movieName);
                var genresIDs = new List<int>();

                foreach (var genre in genres)
                {
                    var genreDb = genresDb.SingleOrDefault(p =>
                        p.Name.Trim().Equals(genre.Trim(), StringComparison.CurrentCultureIgnoreCase));

                    if (genreDb == null) throw new ArgumentException("Genre not found", genre);

                    genresIDs.Add(genreDb.Id);
                }

                await this.movieGenresService.AddManyToMovieAsync(movieDb.Id, genresIDs);
            }
        }

        /// <summary>
        /// Gets the by name asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Movie</returns>
        public Task<Movie> GetByNameAsync(string name)
        {
            return this.moviesRepository.GetAsyncByName(name);
        }


        /// <summary>
        /// Gets the movies names.
        /// </summary>
        /// <returns>List<string/></returns>
        public async Task<List<string>> GetMovieSearchOptions()
        {
            var resultOptions = new List<string>();
            var moviesList = await this.moviesRepository.GetAllAsync();
            var directorsList = moviesList.Select(p => p.Director.Name).Distinct().ToList();
            var movieNamesList = moviesList.Select(p => p.Name).ToList();

            resultOptions.AddRange(movieNamesList);
            resultOptions.AddRange(directorsList);
            // todo add genres

            return resultOptions;
        }

        // azure related task , has to move to another service


        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="processedByAzureJob">if set to <c>true</c> [processed by azure job].</param>
        /// <returns>Movie List.</returns>
        public Task<List<Movie>> GetAll(bool processedByAzureJob = true)
        {
            // todo this property processedByAzureJob not sure if should be there
            var movies = this.moviesRepository
                .GetManyAsync(p => p.ProcessedByAzureJob == processedByAzureJob && p.IsDeleted == false);
            return movies;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="processedByAzureJob">if set to <c>true</c> [processed by azure job].</param>
        /// <returns>Movie List.</returns>
        public Task<List<Movie>> GetAllMoviesImagesFilter(bool processedByAzureJob = true)
        {
            var movies = this.moviesRepository
                .GetManyAsync(p =>
                    p.ProcessedByAzureJob == processedByAzureJob && p.IsDeleted == false &&
                    string.IsNullOrEmpty(p.UrlImage));
            return movies;
        }


        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="processedByAzureJob">if set to <c>true</c> [processed by azure job].</param>
        /// <returns>Movie List.</returns>
        public Task<List<Movie>> GetAllMoviesCoverImagesFilter(bool processedByAzureJob = false)
        {
            var movies = this.moviesRepository
                .GetManyAsync(p =>
                    p.ProcessedByAzureJob == processedByAzureJob && p.IsDeleted == false &&
                    string.IsNullOrEmpty(p.UrlCoverImage));
            return movies;
        }

        private List<MovieModel> SetContinueWatching(
            List<Movie> moviesByCategory,
            List<MovieWatching> moviesWatching)
        {
            var moviesModels = this.mapper.Map<List<Movie>, List<MovieModel>>(moviesByCategory);
            var moviesWatchingIds = moviesWatching.Select(p => p.Movie.Id).ToList();
            foreach (var movie in moviesModels)
            {
                if (moviesWatchingIds.Contains(movie.Id))
                {
                    var movieWatching = moviesWatching.Single(p => p.Movie.Id == movie.Id);
                    movie.RemainingTime = movieWatching.RemainingTime;
                    movie.ElapsedTime = movieWatching.ElapsedTime;
                    movie.Duration = movieWatching.Duration;
                }
            }

            return moviesModels;
        }
    }
}
