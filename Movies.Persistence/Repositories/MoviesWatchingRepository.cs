namespace Movies.Persistence.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Movies.Domain.Entities;
    using Movies.Persistence.Repositories.Base;
    using Movies.Persistence.Repositories.Base.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IMoviesWatchingRepository : IRepository<MovieWatching>
    {
        Task<List<Movie>> GetManyAsync(string userId);
        Task<MovieWatching> GetAsync(string userId, Guid movieId);
    }

    public class MoviesWatchingRepository : RepositoryBase<MovieWatching>, IMoviesWatchingRepository
    {

        private readonly IMoviesRepository moviesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoviesWatchingRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public MoviesWatchingRepository(PolluxMoviesDbContext context, IMoviesRepository moviesRepository)
            : base(context)
        {
            this.moviesRepository = moviesRepository;
        }

        /// <summary>Gets the many asynchronous.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns><List<MovieWatching></returns>
        public async Task<List<Movie>> GetManyAsync(string userId)
        {
            var moviesWatchingDb = await this.dbSet
                .Include(p => p.Movie)
                .Where(p => p.UserId == userId)
                .Select(p => p.Movie)
                .ToListAsync();

            return moviesWatchingDb;
        }


        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>MovieWatching</returns>
        public async Task<MovieWatching> GetAsync(string userId, Guid movieId)
        {
            var moviesWatchingDb = await this.dbSet
                .Include(p => p.Movie)
                .SingleOrDefaultAsync(p => p.UserId == userId && p.MovieId == movieId);

            if (moviesWatchingDb == null)
            {
               var movieDb = await this.moviesRepository.GetAsync(movieId);
               return new MovieWatching() { Movie = movieDb };
            }

            return moviesWatchingDb;
        }

    }
}