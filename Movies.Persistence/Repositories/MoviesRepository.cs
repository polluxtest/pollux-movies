using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Movies.Common.Constants;
using Movies.Domain.Entities;
using Movies.Persistence.Queries;
using Movies.Persistence.Repositories.Base;
using Movies.Persistence.Repositories.Base.Interfaces;

namespace Movies.Persistence.Repositories
{
    /// <summary>
    /// Users Repository contract.
    /// </summary>
    public interface IMoviesRepository : IRepository<Movie>
    {
        Task<MoviesQuery> GetAsync(Guid movieId, string userId);
        Task<Movie> GetAsyncByName(string name);
        Task<List<MoviesQuery>> GetAllAsync(string userId);
        new Task<List<Movie>> GetAllAsync();
        Task<List<MoviesQuery>> SearchAsync(string search, string userId);
        Task<List<MoviesQuery>> GetRecommendedAsync(string userId);
        Task<List<MoviesQuery>> GetRecommendedByPolluxAsync(string userId);
        IQueryable<MoviesQuery> GetBaseQuery(string userId);
        Task<List<string>> GetGenresAsync();
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
        public new Task<List<Movie>> GetAllAsync()
        {
            // todo check optimization this query

            return this.dbSet
                .Include(p=> p.Genre)
                .Include(p => p.Director).ToListAsync();
        }

        /// <summary>
        /// Get Recommended Movies
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns><List<MoviesQueryModel></returns>
        public Task<List<MoviesQuery>> GetRecommendedAsync(string userId)
        {
            var sqlQuery = this.GetBaseQuery(userId).
                OrderByDescending(p => p.Movie.Likes);

            return sqlQuery.Take(MoviesSearchContants.MaxResultSearch).ToListAsync();
        }

        /// <summary>
        /// Get Recommended movies by pollux
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns><List<MoviesQueryModel></returns>
        public Task<List<MoviesQuery>> GetRecommendedByPolluxAsync(string userId)
        {
            var sqlQuery = this.GetBaseQuery(userId).Where(p => p.Movie.Recommended);

            return sqlQuery.Take(MoviesSearchContants.MaxResultSearch).ToListAsync();
        }

        /// <summary>
        /// Searches the specified search.
        /// </summary>
        /// <param name="search">The search string.</param>
        /// <param name="userId">The user Id.</param>
        /// <returns>Movie List SearchAsync Result.</returns>
        public Task<List<MoviesQuery>> SearchAsync(string search, string userId)
        {
            var sqlQuery = this.GetBaseQuery(userId)
                .Where(p => p.Movie.Name.Contains(search) ||
                            p.Movie.Language.Contains(search) ||
                            p.Movie.Director.Name.Contains(search));

            return sqlQuery.ToListAsync();
        }

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task<List<Movie></returns>
        public new Task<List<MoviesQuery>> GetAllAsync(string userId)
        {
            var sqlQuery = this.GetBaseQuery(userId);
            return sqlQuery.ToListAsync();
        }

        /// <summary>
        /// Gets the genres asynchronous.
        /// </summary>
        /// <returns>List<string></returns>
        public async Task<List<string>> GetGenresAsync()
        {
            var genres = await this.dbSet.Select(p => p.Genre.Name).Distinct().ToListAsync();
            return genres;
        }

        /// <summary>
        /// Base Query to get all movies with the neccesary joins
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns>IQueryable<MoviesQueryModel></returns>
        public IQueryable<MoviesQuery> GetBaseQuery(string userId)
        {
            var sqlQuery =
                from movie in this.db.Movies
                join genre in this.db.Genres on movie.GenreId equals genre.Id
                join director in this.db.Directors on movie.DirectorId equals director.Id
                join movieWatching in this.db.MoviesWatching on movie.Id equals movieWatching.MovieId into mg
                from moviesWatching in mg.DefaultIfEmpty()
                where moviesWatching.UserId == userId || moviesWatching.UserId == null
                select new MoviesQuery()
                {
                    Genre = genre.Name,
                    Movie = movie,
                    Director = director,
                    ElapsedTime = moviesWatching != null ? moviesWatching.ElapsedTime : 0,
                    Duration = moviesWatching != null ? moviesWatching.Duration : 0,
                    RemainingTime = moviesWatching != null ? moviesWatching.RemainingTime : 0,
                };


            return sqlQuery;
        }


        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>MoviesQuery</returns>
        public async Task<MoviesQuery> GetAsync(Guid movieId, string userId)
        {
            var sqlQuery =
                from movie in this.db.Movies
                join genre in this.db.Genres on movie.GenreId equals genre.Id
                join director in this.db.Directors on movie.DirectorId equals director.Id
                join movieWatching in this.db.MoviesWatching on movie.Id equals movieWatching.MovieId into mg
                from moviesWatching in mg.DefaultIfEmpty()
                where (moviesWatching.UserId == userId || moviesWatching.UserId == null) && movie.Id == movieId
                select new MoviesQuery()
                {
                    Genre = genre.Name,
                    Movie = movie,
                    Director = director,
                    CategoryGenres = (from movieGenres in this.db.MovieGenres
                        where movieGenres.MovieId == movie.Id
                        select movieGenres.Genre.Name).ToList(),
                    ElapsedTime = moviesWatching != null ? moviesWatching.ElapsedTime : 0,
                    Duration = moviesWatching != null ? moviesWatching.Duration : 0,
                    RemainingTime = moviesWatching != null ? moviesWatching.RemainingTime : 0,
                };


            return await sqlQuery.SingleOrDefaultAsync();
        }

        #region azure

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
                .SingleOrDefaultAsync(p => p.Name.Trim().Equals(name.Trim()));

            if (movieDb == null) throw new ArgumentException("Movie not found", name);

            return movieDb;
        }

        #endregion
    }
}
