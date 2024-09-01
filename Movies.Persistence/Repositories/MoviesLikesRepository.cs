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
    public interface IUserLikesRepository : IRepository<MoviesLikes>
    {
        Task<List<Guid>> GetLikesMoviesIds(Guid userId);
    }

    public class MoviesLikesRepository : RepositoryBase<MoviesLikes>, IUserLikesRepository
    {
        public MoviesLikesRepository(PolluxMoviesDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Gets the movies list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Movie Ids List.</returns>
        public async Task<List<Guid>> GetLikesMoviesIds(Guid userId)
        {
            return await this.dbSet
                .Where(p => p.UserId.Equals(userId))
                .Select(p => p.MovieId)
                .ToListAsync();
        }
    }
}
