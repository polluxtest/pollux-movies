using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories.Base;
using Movies.Persistence.Repositories.Base.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Movies.Persistence.Repositories
{
    public interface IUserMoviesRepository : IRepository<UserMovies>
    {
        Task<List<Guid>> GetMoviesListIds(string userId);
        Task<List<Movie>> GetMoviesMyList(string userId);
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
        public async Task<List<Guid>> GetMoviesListIds(string userId)
        {
            return this.dbSet.Where(p => p.UserId.ToString().Equals(userId.ToUpper()))
            .Select(p => p.MovieId).ToList();
        }

        /// <summary>
        /// Gets the movies my list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List Movie.</returns>
        public async Task<List<Movie>> GetMoviesMyList(string userId)
        {
            return this.dbSet
                .Include(p => p.Movie)
                .ThenInclude(p => p.Director)
                .Where(p => p.UserId.ToString() == userId.ToUpper())
                .Select(p => p.Movie)
                .ToList();
        }
    }
}
