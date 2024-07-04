using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Movies.Persistence.Repositories.Base.Interfaces;
using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Persistence.QueryResults;
using Movies.Persistence.Repositories.Base;

namespace Movies.Persistence.Repositories
{
    public interface IMoviesByGenresRepository : IRepository<MovieGenres>
    {
        Task<List<MoviesQueryResult>> GetAllAsync(string userId);
        Task<List<string>> GetGenresByMovieIdAsync(Guid movieId);
        Task<List<MoviesQueryResult>> GetSearchByGenreAsync(string userId, string genre);
        Task<List<string>> GetGenresAsync();
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
        /// <param name="genre">The genre.</param>
        /// <returns>List<MoviesQueryResult></returns>
        public new Task<List<MoviesQueryResult>> GetSearchByGenreAsync(string userId, string genre)
        {
            var sqlQuery =
                from movieGenre in this.db.MovieGenres
                join genres in this.db.Genres on movieGenre.GenreGenericId equals genres.Id
                join movie in this.db.Movies on movieGenre.MovieId equals movie.Id
                join director in this.db.Directors on movie.DirectorId equals director.Id
                join movieWatching in this.db.MoviesWatching on movie.Id equals movieWatching.MovieId into mg
                from moviesWatching in mg.DefaultIfEmpty()
                where (moviesWatching.UserId == userId ||
                       moviesWatching.UserId == null) && movieGenre.GenreGeneric.Name == genre
                select new MoviesQueryResult()
                {
                    Genre = genres,
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
        /// <returns>List<MoviesQueryResult></returns>
        public new Task<List<MoviesQueryResult>> GetAllAsync(string userId)
        {
            var sqlQuery =
                from movieGenre in this.db.MovieGenres
                join genre in this.db.Genres on movieGenre.GenreId equals genre.Id
                join movie in this.db.Movies on movieGenre.MovieId equals movie.Id
                join director in this.db.Directors on movie.DirectorId equals director.Id
                join movieWatching in this.db.MoviesWatching on movie.Id equals movieWatching.MovieId into mg
                from moviesWatching in mg.DefaultIfEmpty()
                where moviesWatching.UserId == userId || moviesWatching.UserId == null
                select new MoviesQueryResult()
                {
                    Genre = genre,
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

        /// <summary>
        /// Gets the genres asynchronous.
        /// </summary>
        /// <returns>List<string></returns>
        public async Task<List<string>> GetGenresAsync()
        {
            var genres = await this.dbSet.Select(p => p.GenreGeneric.Name).Distinct().ToListAsync();
            return genres;
        }
    }
}
