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
    public interface IMoviesListRepository : IRepository<MoviesLists>
    {
        Task<List<Guid>> GetMoviesListIds(string userId);
        Task<List<Movie>> GetMoviesMyList(string userId);
    }

    public class MoviesListRepository : RepositoryBase<MoviesLists>, IMoviesListRepository
    {
        public MoviesListRepository(PolluxMoviesDbContext moviesDbContext)
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
            return await this.dbSet.Where(p => p.UserId.ToString().Equals(userId))
                .Select(p => p.MovieId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the movies my list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List Movie.</returns>
        public async Task<List<Movie>> GetMoviesMyList(string userId)
        {
            return await this.dbSet
                .Include(p => p.Movie)
                .ThenInclude(p => p.Director)
                .Where(p => p.UserId.ToString() == userId)
                .Select(p => p.Movie)
                .ToListAsync();
        }
    }
}
