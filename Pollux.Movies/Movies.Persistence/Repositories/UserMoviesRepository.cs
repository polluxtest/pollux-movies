using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories.Base;
using Movies.Persistence.Repositories.Base.Interfaces;

namespace Movies.Persistence.Repositories
{
    public interface IUserMoviesRepository : IRepository<UserMovies>
    {
        Task<List<int>> GetMoviesList(string userId);
    }

    public class UserMoviesRepository : RepositoryBase<UserMovies>, IUserMoviesRepository
    {
        public UserMoviesRepository(PolluxMoviesDbContext moviesDbContext)
           : base(moviesDbContext)
        {
        }

        /// <summary>
        /// Gets the movies list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Movie Ids List.</returns>
        public async Task<List<int>> GetMoviesList(string userId)
        {
            return this.dbSet.Where(p => p.UserId.ToString() == userId).Select(p => p.MovieId).ToList();
        }
    }
}
