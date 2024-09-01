using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movies.Persistence.Repositories.Base.Interfaces;
using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Persistence.Queries;
using Movies.Persistence.Repositories.Base;

namespace Movies.Persistence.Repositories
{
    public interface IMoviesByGenresRepository : IRepository<MovieGenres>
    {
        Task<List<MoviesQuery>> GetAllAsync(string userId);
        Task<List<string>> GetGenresByMovieIdAsync(Guid movieId);
        Task<List<MoviesQuery>> GetSearchByCategoryAsync(string userId, string genre);
    }

    public class MovieByGenresRepository : RepositoryBase<MovieGenres>, IMoviesByGenresRepository
    {
        private readonly PolluxMoviesDbContext db;

        public MovieByGenresRepository(PolluxMoviesDbContext context)
            : base(context)
        {
            this.db = context;
        }

        /// <summary>
        /// Gets the search by genre asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="genreName">The genre.</param>
        /// <returns>List<MoviesQueryModel></returns>
        public new Task<List<MoviesQuery>> GetSearchByCategoryAsync(string userId, string genreName)
        {
            var sqlQuery =
                from movie in this.db.Movies
                join genre in this.db.Genres on movie.GenreId equals genre.Id
                join director in this.db.Directors on movie.DirectorId equals director.Id
                join movieWatching in this.db.MoviesWatching on movie.Id equals movieWatching.MovieId into mg
                from moviesWatching in mg.DefaultIfEmpty()
                where (moviesWatching.UserId == userId || moviesWatching.UserId == null) && genre.Name == genreName
                select new MoviesQuery()
                {
                    Genre = genre.Name,
                    Movie = movie,
                    Director = director,
                    ElapsedTime = moviesWatching != null ? moviesWatching.ElapsedTime : 0,
                    Duration = moviesWatching != null ? moviesWatching.Duration : 0,
                    RemainingTime = moviesWatching != null ? moviesWatching.RemainingTime : 0,
                };

            return sqlQuery.ToListAsync();
        }

        /// <summary>
        /// Gets all asynchronous filter by genre.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List<MoviesQueryModel></returns>
        public new Task<List<MoviesQuery>> GetAllAsync(string userId)
        {
            var sqlQuery =
                from movieGenres in this.db.MovieGenres
                join categorGenre in this.db.Genres on movieGenres.GenreId equals categorGenre.Id
                join movie in this.db.Movies on movieGenres.MovieId equals movie.Id
                join genre in this.db.Genres on movie.GenreId equals genre.Id
                join director in this.db.Directors on movie.DirectorId equals director.Id
                join movieWatching in this.db.MoviesWatching on movie.Id equals movieWatching.MovieId into mg
                from moviesWatching in mg.DefaultIfEmpty()
                select new MoviesQuery()
                {
                    Genre = genre.Name,
                    CategoryGenres = new List<string>() { categorGenre.Name },
                    Movie = movie,
                    Director = director,
                    ElapsedTime = moviesWatching != null ? moviesWatching.ElapsedTime : 0,
                    Duration = moviesWatching != null ? moviesWatching.Duration : 0,
                    RemainingTime = moviesWatching != null ? moviesWatching.RemainingTime : 0,
                };

            return sqlQuery.ToListAsync();
        }

        /// <summary>
        /// Gets the genres by movie identifier asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>List<string/></returns>
        public async Task<List<string>> GetGenresByMovieIdAsync(Guid movieId)
        {
            return await this.dbSet
                .Where(p => p.MovieId == movieId)
                .Include(p => p.Genre)
                .Select(p => p.Genre.Name)
                .ToListAsync();
        }
    }
}
