using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.Models;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;
using Movies.Application.ExtensionMethods;
using Movies.Application.Mappers;

namespace Movies.Application
{
    public interface IMoviesByGenresService
    {
        Task AddManyToMovieAsync(Guid movieId, List<int> genresIds);
        Task<List<MoviesByCategoryModel>> GetAllByGenreAsync(string userId, string sortBy = null);
        Task<List<string>> GetAllByMovieIdAsync(Guid movieId);
    }

    public class MoviesByGenresService : IMoviesByGenresService
    {
        private readonly IMoviesByGenresRepository moviesByGenreRepository;
        private readonly IMapper mapper;
        private readonly IMoviesWatchingService moviesWatchingService;

        public MoviesByGenresService(
            IMoviesByGenresRepository moviesByGenreRepository,
            IMoviesWatchingService moviesWatchingService,
            IMapper mapper)
        {
            this.moviesByGenreRepository = moviesByGenreRepository;
            this.moviesWatchingService = moviesWatchingService;
            this.mapper = mapper;
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

        /// <summary>
        /// Gets all by genre asynchronous.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>List<MoviesByCategoryModel/></returns>
        public async Task<List<MoviesByCategoryModel>> GetAllByGenreAsync(string userId, string sortBy = null)
        {
            var moviesByGenreDb = await this.moviesByGenreRepository.GetAllAsync();
            var moviesWatchingDb = await this.moviesWatchingService.GetAllAsync(userId);
            var moviesByGenre = this.SetContinueWatching(moviesByGenreDb, moviesWatchingDb);

            return moviesByGenre
                .GroupBy(p => p.Genre.Name)
                .Select(x =>
                {
                    var moviesByCategory = new MoviesByCategoryModel()
                    {
                        Title = x.Key,
                        Movies = x.Select(p => p.Movie).ToList().SortCustomBy(sortBy),
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


        private List<MovieGenreModel> SetContinueWatching(
            List<MovieGenres> moviesByCategory,
            List<MovieWatching> moviesWatching)
        {
            var moviesWatchingIds = moviesWatching.Select(p => p.Movie.Id).ToList();
            var moviesModels = this.mapper.Map<List<MovieGenres>, List<MovieGenreModel>>(moviesByCategory);
            foreach (var movie in moviesModels)
            {
                if (moviesWatchingIds.Contains(movie.Movie.Id))
                {
                    var movieWatching = moviesWatching.Single(p => p.Movie.Id == movie.Movie.Id);

                    movie.Movie.RemainingTime = movieWatching.RemainingTime;
                    movie.Movie.ElapsedTime = movieWatching.ElapsedTime;
                    movie.Movie.Duration = movieWatching.Duration;
                }
            }

            return moviesModels;
        }
    }
}
