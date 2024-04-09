using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories.Base;
using Movies.Persistence.Repositories.Base.Interfaces;

namespace Movies.Persistence.Repositories
{
    public interface IMoviesWatchingRepository : IRepository<MovieWatching>
    {
        Task<List<MovieWatching>> GetManyAsync(string userId);
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
        public async Task<List<MovieWatching>> GetManyAsync(string userId)
        {
            var moviesWatchingDb = await this.dbSet
                .Include(p => p.Movie)
                .ThenInclude(p => p.Director)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return moviesWatchingDb;
        }
    }
}