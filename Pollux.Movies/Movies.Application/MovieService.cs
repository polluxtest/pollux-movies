using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.Models;
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

        Task<List<Movie>> GetAllImages(bool processedByAzureJob = false);

        Task<List<MoviesByCategoryModel>> GetByLanguage(string sortBy = null);

        Task<List<MoviesByCategoryModel>> GetByDirector(string sortBy = null);

        Task UpdateMovie(Movie movie);

        Task Add(Movie movie);

        Task<List<MoviesByCategoryModel>> Search(string search);

        Task<List<MoviesByCategoryModel>> GetRecommendedByUsers();

        Task<List<MoviesByCategoryModel>> GetRecommendedByPollux();

        Task<MovieInfoModel> GetAsync(int movieId, string userId);

        Task<Movie> GetByNameAsync(string name);

        Task<Movie> GetAsync(int movieId);

    }

    public class MoviesService : IMoviesService
    {
        private readonly IMoviesRepository moviesRepository;
        private readonly IUserMoviesService userMoviesService;
        private readonly IUserLikesService userMovieLikesService;
        private readonly IMapper mapper;

        public MoviesService(
            IMoviesRepository moviesRepository,
            IUserMoviesService userMoviesService,
            IUserLikesService userMovieLikesService,
            IMapper mapper)
        {
            this.moviesRepository = moviesRepository;
            this.userMoviesService = userMoviesService;
            this.userMovieLikesService = userMovieLikesService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="processedByAzureJob">if set to <c>true</c> [processed by azure job].</param>
        /// <returns>Movie List.</returns>
        public Task<List<Movie>> GetAll(bool processedByAzureJob = false)
        {
            var movies = this.moviesRepository
                .GetManyAsync(p => p.ProcessedByAzureJob == processedByAzureJob && p.IsDeleted == false);
            return movies;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="processedByAzureJob">if set to <c>true</c> [processed by azure job].</param>
        /// <returns>Movie List.</returns>
        public Task<List<Movie>> GetAllImages(bool processedByAzureJob = false)
        {
            var movies = this.moviesRepository
                .GetManyAsync(p => p.ProcessedByAzureJob == processedByAzureJob && p.IsDeleted == false && string.IsNullOrEmpty(p.UrlImage));
            return movies;
        }

        /// <summary>
        /// Gets the by language.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>MovieModel List by Language.</returns>
        public async Task<List<MoviesByCategoryModel>> GetByLanguage(string sortBy = null)
        {
            var moviesDb = await this.moviesRepository.GetAll();

            var moviesGroupedByLanguage = moviesDb
                .GroupBy(p => p.Language)
                .Select(p => new MoviesByCategoryModel()
                {
                    Title = $"{p.Key} {TitleConstants.Language}",
                    Movies = this.mapper.Map<List<Movie>, List<MovieModel>>(p.ToList()),
                });

            var movies = moviesGroupedByLanguage.ToList();
            await this.SortMovies(ref movies, sortBy);

            return movies;
        }

        /// <summary>
        /// Gets the by director.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>MovieModel List by Direactor.</returns>
        public async Task<List<MoviesByCategoryModel>> GetByDirector(string sortBy = null)
        {
            var moviesDb = await this.moviesRepository.GetAll();

            var moviesGroupedByDirector = moviesDb
                .GroupBy(p => p.Director.Name)
                .Select(p => new MoviesByCategoryModel()
                {
                    Title = p.Key,
                    Movies = this.mapper.Map<List<Movie>, List<MovieModel>>(p.ToList()),
                });

            var movies = moviesGroupedByDirector.ToList();
            await this.SortMovies(ref movies, sortBy);

            return movies;
        }

        /// <summary>
        /// Sorts the movies.
        /// </summary>
        /// <param name="movies">The movies.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>Movies Sorted.</returns>
        public Task<List<MoviesByCategoryModel>> SortMovies(ref List<MoviesByCategoryModel> movies, string sortBy = null)
        {
            if (string.IsNullOrEmpty(sortBy)) return Task.FromResult(movies);

            switch (sortBy)
            {
                case SortByConstants.AlphaAscending: movies = movies.OrderBy(p => p.Title).ToList(); break;
                case SortByConstants.AlphaDescending: movies = movies.OrderByDescending(p => p.Title).ToList(); break;
            }

            return Task.FromResult(movies);
        }

        /// <summary>
        /// Updates the specified movie.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns>Task.</returns>
        public Task UpdateMovie(Movie movie)
        {
            this.moviesRepository.Update(movie);
            this.moviesRepository.Save();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds the specified movie.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns>Task.</returns>
        public Task Add(Movie movie)
        {
            this.moviesRepository.Add(movie);
            this.moviesRepository.Save();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Searches the specified search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns>List<MoviesByCategoryModel>.</returns>
        public async Task<List<MoviesByCategoryModel>> Search(string search)
        {
            var moviesDbSearch = await this.moviesRepository.Search(search);
            var movies = this.mapper.Map<List<Movie>, List<MovieModel>>(moviesDbSearch);

            return new List<MoviesByCategoryModel>() { new MoviesByCategoryModel() { Movies = movies, Title = TitleConstants.Search } };
        }

        /// <summary>
        /// Gets the recommended by users.
        /// </summary>
        /// <returns>List<MoviesByCategoryModel/>.</returns>
        public async Task<List<MoviesByCategoryModel>> GetRecommendedByPollux()
        {
            var moviesDb = await this.moviesRepository
                .GetManyAsync(p => p.ProcessedByAzureJob == true && p.IsDeleted == false && p.Recommended);

            var movies = this.mapper.Map<List<Movie>, List<MovieModel>>(moviesDb);

            return new List<MoviesByCategoryModel>() { new MoviesByCategoryModel() { Movies = movies, Title = TitleConstants.RecommendedByPollux } };
        }

        /// <summary>
        /// Gets the recommended by users.
        /// </summary>
        /// <returns>List<MoviesByCategoryModel/>.</returns>
        public async Task<List<MoviesByCategoryModel>> GetRecommendedByUsers()
        {
            var moviesDb = await this.moviesRepository.GetRecommended();

            var movies = this.mapper.Map<List<Movie>, List<MovieModel>>(moviesDb);

            return new List<MoviesByCategoryModel>() { new MoviesByCategoryModel() { Movies = movies, Title = TitleConstants.RecommendedByUsers } };

        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>MovieInfoModel.</returns>
        public async Task<MovieInfoModel> GetAsync(int movieId, string userId)
        {
            var movieInfoModel = new MovieInfoModel();
            var movieDb = await this.moviesRepository.GetAsync(movieId);

            Throw.When(movieDb == null, new ArgumentException($"movie not found id {movieId}"));

            movieInfoModel.IsInList = await this.userMoviesService.IsMovieInListByUser(movieId, userId);
            movieInfoModel.IsLiked = await this.userMovieLikesService.IsMovieLikedByUser(movieId, userId);

            return this.mapper.Map<Movie, MovieInfoModel>(movieDb, movieInfoModel);
        }

        /// <summary>
        /// Gets the by name asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Movie</returns>
        public Task<Movie> GetByNameAsync(string name)
        {
            return this.moviesRepository.GetAsync(p => p.Name.TrimAll().ToLower().Equals(name));
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        public Task<Movie> GetAsync(int movieId)
        {
            return this.moviesRepository.GetAsync(p => p.Id == movieId);
        }
    }
}
