namespace Movies.Persistence.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Movies.Domain.Entities;
    using Movies.Persistence.Repositories.Base;
    using Movies.Persistence.Repositories.Base.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IMoviesWatchingRepository : IRepository<MovieWatching>
    {
        Task<List<Movie>> GetManyAsync(string userId);
    }

    public class MoviesWatchingRepository : RepositoryBase<MovieWatching>, IMoviesWatchingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoviesWatchingRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public MoviesWatchingRepository(PolluxMoviesDbContext context)
            : base(context)
        {
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
    }
}