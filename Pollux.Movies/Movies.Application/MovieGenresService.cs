using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.Models;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;

namespace Movies.Application
{
    public interface IMovieGenresService
    {
        Task AddManyToMovieAsync(Guid movieId, List<int> genresIds);
        Task<List<MoviesByCategoryModel>> GetAllByGenreAsync();
    }

    public class MovieGenresService : IMovieGenresService
    {
        private readonly IMovieGenresRepository movieGenreRepository;
        private readonly IMapper mapper;

        public MovieGenresService(
            IMovieGenresRepository movieGenreRepository,
            IMapper mapper)
        {
            this.movieGenreRepository = movieGenreRepository;
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
                await this.movieGenreRepository.AddASync(new MovieGenres()
                {
                    MovieId = movieId,
                    GenreId = genreId,
                });
            }

            this.movieGenreRepository.Save();
        }

        /// <summary>
        /// Gets all by genre asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<List<MoviesByCategoryModel>> GetAllByGenreAsync()
        {
            var moviesByGenreDb = await this.movieGenreRepository.GetAllAsync();

            return moviesByGenreDb
                .GroupBy(p => p.Genre.Name)
                .Select(x => new MoviesByCategoryModel()
                {
                    Title = x.Key,
                    Movies = this.mapper.Map<List<Movie>, List<MovieModel>>(x.Select(m => m.Movie).OrderBy(p => p.Name).ToList()),
                })
                .ToList();
        }

        /// <summary>
        /// Deletes all by movie identifier.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>Task.</returns>
        private async Task DeleteAllByMovieId(Guid movieId)
        {
            var movieGenres = await this.movieGenreRepository.GetManyAsync(p => p.MovieId == movieId);
            foreach (var movieGenre in movieGenres)
            {
                this.movieGenreRepository.Delete(movieGenre);
            }

            this.movieGenreRepository.Save();
        }
    }
}
