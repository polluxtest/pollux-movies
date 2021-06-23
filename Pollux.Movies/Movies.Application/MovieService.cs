using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.Models;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;

namespace Movies.Application
{
    public interface IMoviesService
    {
        public Task<List<Movie>> GetAll(bool processedByAzureJob = false);

        public Task<List<Movie>> GetAllImages(bool processedByAzureJob = false);

        public Task<List<MoviesByCategoryModel>> GetByLanguage();

        Task UpdateMovie(Movie movie);

        Task Add(Movie movie);
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
        /// <returns>MovieModel List by Language.</returns>
        public async Task<List<MoviesByCategoryModel>> GetByLanguage()
        {
            var moviesDb = await this.moviesRepository.GetAll();

            var moviesGroupedByLanguage = moviesDb
                .GroupBy(p => p.Language)
                .Select(p => new MoviesByCategoryModel()
                {
                    Title = p.Key,
                    Movies = this.mapper.Map<List<Movie>, List<MovieModel>>(p.ToList()),
                });

            return moviesGroupedByLanguage.ToList();
        }

        /// <summary>
        /// Gets all by director.
        /// </summary>
        /// <returns>MovieModel List by Direactor.</returns>
        public async Task<List<MoviesByCategoryModel>> GetAllByDirector()
        {
            var moviesDb = await this.moviesRepository.GetAll();

            var moviesGroupedByDirector = moviesDb
                .GroupBy(p => p.Director.Name)
                .Select(p => new MoviesByCategoryModel()
                {
                    Title = p.Key,
                    Movies = this.mapper.Map<List<Movie>, List<MovieModel>>(p.ToList()),
                });

            return moviesGroupedByDirector.ToList();
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
    }
}
