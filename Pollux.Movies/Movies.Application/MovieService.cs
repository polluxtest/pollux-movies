using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.Models;
using Movies.Common.Constants.Strings;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;

namespace Movies.Application
{
    public interface IMoviesService
    {
        public Task<List<Movie>> GetAll(bool processedByAzureJob = false);

        public Task<List<Movie>> GetAllImages(bool processedByAzureJob = false);

        public Task<List<MoviesByCategoryModel>> GetByLanguage(string sortBy = null);

        Task<List<MoviesByCategoryModel>> GetByDirector(string sortBy = null);

        Task UpdateMovie(Movie movie);

        Task Add(Movie movie);

        Task<List<MoviesByCategoryModel>> Search(string search);

        Task<Movie> GetAsync(int movieId);
    }

    public class MoviesService : IMoviesService
    {
        private readonly IMoviesRepository moviesRepository;
        private readonly IMapper mapper;

        public MoviesService(
            IMoviesRepository moviesRepository,
            IMapper mapper)
        {
            this.moviesRepository = moviesRepository;
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
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>MovieModel.</returns>
        public async Task<Movie> GetAsync(int movieId)
        {
            return await this.moviesRepository.GetAsync(p => p.Id == movieId);
        }
    }
}
