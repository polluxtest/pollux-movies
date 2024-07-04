using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.ExtensionMethods;
using Movies.Application.Models;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;
using Movies.Persistence.QueryResults;

namespace Movies.Application
{
    public interface IMoviesByGenresService
    {
        Task AddManyToMovieAsync(Guid movieId, List<int> genresIds);
        Task<List<MoviesByCategoryModel>> GetAllByGenreAsync(string userId, string sortBy = null);
        Task<List<string>> GetAllByMovieIdAsync(Guid movieId);
        Task<List<string>> GetGenresAsync();
        Task<List<MovieModel>> GetSearchByGenreAsync(string userId, string genre, string sortBy = null);
    }

    public class MoviesByGenresService : IMoviesByGenresService
    {
        private readonly IMoviesByGenresRepository moviesByGenreRepository;
        private readonly IMapper mapper;

        public MoviesByGenresService(
            IMoviesByGenresRepository moviesByGenreRepository,
            IMapper mapper)
        {
            this.moviesByGenreRepository = moviesByGenreRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets all by genre asynchronous.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>List<MoviesByCategoryModel/></returns>
        public async Task<List<MoviesByCategoryModel>> GetAllByGenreAsync(string userId, string sortBy = null)
        {
            var moviesByGenreDb = await this.moviesByGenreRepository.GetAllAsync(userId);
            return moviesByGenreDb
                .GroupBy(p => p.Genre.Name)
                .Select(x =>
                {
                    var moviesByCategory = new MoviesByCategoryModel()
                    {
                        Title = x.Key,
                        Movies = this.mapper.Map<List<MoviesQueryResult>, List<MovieModel>>(x.ToList())
                            .SortCustomBy(sortBy),
                    };

                    return moviesByCategory;
                }).ToList();
        }

        /// <summary>
        /// Gets all by movie identifier asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>List<string/></returns>
        public async Task<List<string>> GetAllByMovieIdAsync(Guid movieId)
        {
            return await this.moviesByGenreRepository.GetGenresByMovieIdAsync(movieId);
        }

        /// <summary>
        /// Gets the genres asynchronous.
        /// </summary>
        /// <returns>List<string></returns>
        public async Task<List<string>> GetGenresAsync()
        {
            return await this.moviesByGenreRepository.GetGenresAsync();
        }

        /// <summary>
        /// Gets the search by genre asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="genre">The genre.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>Task<List<MovieModel></returns>
        public async Task<List<MovieModel>> GetSearchByGenreAsync(string userId, string genre, string sortBy = null)
        {
            var moviesByGenreDb = await this.moviesByGenreRepository.GetSearchByGenreAsync(userId, genre);
            var moviesSearch = this.mapper.Map<List<MoviesQueryResult>, List<MovieModel>>(moviesByGenreDb);
            return moviesSearch.SortCustomBy(sortBy);
        }

        #region azure

        /// <summary>
        /// Deletes all by movie identifier.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>Task.</returns>
        private async Task DeleteAllByMovieId(Guid movieId)
        {
            var movieGenres = await this.moviesByGenreRepository.GetManyAsync(p => p.MovieId == movieId);
            foreach (var movieGenre in movieGenres)
            {
                this.moviesByGenreRepository.Delete(movieGenre);
            }

            await this.moviesByGenreRepository.SaveAsync();
        }

        /// <summary>
        /// Adds the many to movie asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="genresIds">The genres ids.</param>
        public async Task AddManyToMovieAsync(Guid movieId, List<int> genresIds)
        {
            await this.DeleteAllByMovieId(movieId);

            foreach (var genreId in genresIds)
            {
                await this.moviesByGenreRepository.AddAsync(new MovieGenres()
                {
                    MovieId = movieId,
                    GenreId = genreId,
                });
            }

            await this.moviesByGenreRepository.SaveAsync();
        }

        #endregion
    }
}
