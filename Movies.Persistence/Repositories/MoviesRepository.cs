using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Persistence.QueryResults;
using Movies.Persistence.Repositories.Base;
using Movies.Persistence.Repositories.Base.Interfaces;

namespace Movies.Persistence.Repositories
{
    /// <summary>
    /// Users Repository contract.
    /// </summary>
    public interface IMoviesRepository : IRepository<Movie>
    {
        Task<MovieWatching> GetAsync(Guid movieId, string userId);
        Task<Movie> GetAsyncByName(string name);
        Task<List<MoviesQueryResult>> GetAllAsync(string userId);
        new Task<List<Movie>> GetAllAsync();
        Task<List<MoviesQueryResult>> Search(string search, string userId);
        Task<List<Movie>> GetRecommendedAsync();
        Task<List<Movie>> GetRecommendedByPolluxAsync();
    }

    /// <summary>
    /// Users Repository Data.
    /// </summary>
    public class MoviesRepository : RepositoryBase<Movie>, IMoviesRepository
    {
        private readonly PolluxMoviesDbContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersRepository"/> class.
        /// </summary>
        /// <param name="moviesDbContext">The database context.</param>
        public MoviesRepository(PolluxMoviesDbContext moviesDbContext)
            : base(moviesDbContext)
        {
            this.db = moviesDbContext;
        }

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task<List<Movie></returns>
        public new Task<List<MoviesQueryResult>> GetAllAsync(string userId)
        {
            var sqlQuery =
                from movie in this.db.Movies
                join director in this.db.Directors on movie.DirectorId equals director.Id
                join movieWatching in this.db.MoviesWatching on movie.Id equals movieWatching.MovieId into mg
                from moviesWatching in mg.DefaultIfEmpty()
                where moviesWatching.UserId == userId
                select new MoviesQueryResult()
                {
                    Movie = movie,
                    Director = director,
                    ElapsedTime = moviesWatching != null ? moviesWatching.ElapsedTime : 0,
                    Duration = moviesWatching != null ? moviesWatching.Duration : 0,
                    RemainingTime = moviesWatching != null ? moviesWatching.RemainingTime : 0,
                };

            return sqlQuery.ToListAsync();
        }

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task<List<Movie></returns>
        public new Task<List<Movie>> GetAllAsync()
        {
            return this.dbSet.Include(p => p.Director).ToListAsync();
        }

        /// <summary>
        /// Gets the movie asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="userId">The userId identifier.</param>
        /// <returns>Movie.</returns>
        public new Task<MovieWatching> GetAsync(Guid movieId,string userId)
        {
            var sqlQuery =
                from movie in this.db.Movies
                join director in this.db.Directors on movie.DirectorId equals director.Id
                join movieWatching in this.db.MoviesWatching on movie.Id equals movieWatching.MovieId into mg
                from moviesWatching in mg.DefaultIfEmpty()
                where movie.Id == movieId && moviesWatching.UserId == null || moviesWatching.UserId == userId
                select new MovieWatching()
                {
                    Movie = movie,
                    ElapsedTime = moviesWatching != null ? moviesWatching.ElapsedTime : 0,
                    Duration = moviesWatching != null ? moviesWatching.Duration : 0,
                    RemainingTime = moviesWatching != null ? moviesWatching.RemainingTime : 0,
                };

            return sqlQuery.SingleOrDefaultAsync();
        }

        /// <summary>
        /// Gets the name of the asynchronous by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Movie.</returns>
        /// <exception cref="System.ArgumentException">Movie not found</exception>
        public async Task<Movie> GetAsyncByName(string name)
        {
            var movieDb = await this.dbSet
                .Include(p => p.Director)
                .SingleOrDefaultAsync(p => p.Name.Trim().Equals(name.Trim()) && p.ProcessedByStreamVideo);

            if (movieDb == null) throw new ArgumentException("Movie not found", name);

            return movieDb;
        }

        /// <summary>
        /// Gets the recommended.
        /// </summary>
        /// <returns>List<Movie/></returns>
        public Task<List<Movie>> GetRecommendedAsync()
        {
            return this.dbSet.Include(p => p.Director)
                .OrderBy(p => p.Likes)
                .Take(15)
                .ToListAsync();
        }


        /// <summary>
        /// Gets the recommended by pollux.
        /// </summary>
        /// <returns>Task<List<Movie>></returns>
        public async Task<List<Movie>> GetRecommendedByPolluxAsync()
        {
            return await this.dbSet.Include(p => p.Director)
                .Where(p => p.Recommended)
                .ToListAsync();
        }

        /// <summary>
        /// Searches the specified search.
        /// </summary>
        /// <param name="search">The search string.</param>
        /// <param name="userId">The user Id.</param>
        /// <returns>Movie List Search Result.</returns>
        public Task<List<MoviesQueryResult>> Search(string search, string userId)
        {
            var sqlQuery =
                from movie in this.db.Movies
                join director in this.db.Directors on movie.DirectorId equals director.Id
                join movieWatching in this.db.MoviesWatching on movie.Id equals movieWatching.MovieId into mg
                from moviesWatching in mg.DefaultIfEmpty()
                where moviesWatching.UserId == userId  &&
                    (movie.Name.Contains(search) || director.Name.Contains(search) || movie.Language.Contains(search))
                select new MoviesQueryResult()
                {
                    Movie = movie,
                    Director = director,
                    ElapsedTime = moviesWatching != null ? moviesWatching.ElapsedTime : 0,
                    Duration = moviesWatching != null ? moviesWatching.Duration : 0,
                    RemainingTime = moviesWatching != null ? moviesWatching.RemainingTime : 0,
                };

            return sqlQuery.ToListAsync();
        }

    }
}
