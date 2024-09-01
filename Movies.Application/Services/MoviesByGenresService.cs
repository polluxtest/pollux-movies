namespace Movies.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Movies.Application.ExtensionMethods;
    using Movies.Common.Models;
    using Movies.Domain.Entities;
    using Movies.Persistence.Queries;
    using Movies.Persistence.Repositories;

    public interface IMoviesByGenresService
    {
        Task AddManyToMovieAsync(Guid movieId, List<int> genresIds);

        Task<List<MoviesByCategoryModel>> GetAllByCategoryGenres(string userId, string sortBy = null);

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
        public async Task<List<MoviesByCategoryModel>> GetAllByCategoryGenres(string userId, string sortBy = null)
        {
            var moviesByGenreDb = await this.moviesByGenreRepository.GetAllAsync(userId);
            return moviesByGenreDb
                .GroupBy(p => p.CategoryGenres.FirstOrDefault())
                .Select(x =>
                {
                    var moviesByCategory = new MoviesByCategoryModel()
                    {
                        Title = x.Key,
                        Movies = this.mapper.Map<List<MoviesQuery>, List<MovieModel>>(x.ToList())
                            .SortCustomBy(sortBy),
                    };

                    return moviesByCategory;
                }).ToList();
        }


        /// <summary>
        /// Gets the search by genre asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="genre">The genre.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>Task<List<MovieModel></returns>
        public async Task<List<MovieModel>> GetSearchByGenreAsync(
            string userId,
            string genre,
            string sortBy = null)
        {
            var moviesByGenreDb = await this.moviesByGenreRepository.GetSearchByCategoryAsync(userId, genre);

            var moviesSearch = this.mapper.Map<List<MoviesQuery>, List<MovieModel>>(moviesByGenreDb);

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