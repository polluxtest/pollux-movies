﻿using System.Collections.Generic;
using System.Diagnostics;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="MoviesWatchingRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="moviesRepository">The movies repository.</param>
        public MoviesWatchingRepository(PolluxMoviesDbContext context)
            : base(context)
        {
        }

        /// <summary>Gets the many asynchronous.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns><List<MovieWatching></returns>
        public async Task<List<MovieWatching>> GetManyAsync(string userId)
        {
            var moviesWatchingDb = this.dbSet
                .Include(p => p.Movie)
                .ThenInclude(p => p.Director)
                .Where(p => p.UserId == userId);

            return await moviesWatchingDb.ToListAsync();
        }
    }
}