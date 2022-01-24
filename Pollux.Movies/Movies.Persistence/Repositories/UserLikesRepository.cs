using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories.Base;
using Movies.Persistence.Repositories.Base.Interfaces;

namespace Movies.Persistence.Repositories
{
    public interface IUserLikesRepository : IRepository<UserLikes>
    {
        Task<List<Guid>> GetLikesMoviesIds(string userId);
    }

    public class UserLikesRepository : RepositoryBase<UserLikes>, IUserLikesRepository
    {
        public UserLikesRepository(PolluxMoviesDbContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Gets the movies list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Movie Ids List.</returns>
        public async Task<List<Guid>> GetLikesMoviesIds(string userId)
        {
            return this.dbSet.Where(p => p.UserId.ToString().Equals(userId.ToUpper()))
            .Select(p => p.MovieId).ToList();
        }
    }
}
