using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories.Base;
using Movies.Persistence.Repositories.Base.Interfaces;

namespace Movies.Persistence.Repositories
{
    /// <summary>
    /// Users Repository contract.
    /// </summary>
    public interface IMoviesRepository : IRepository<Movie>
    {
        public Task<List<Movie>> GetAll();
        public Task<List<Movie>> Search(string search);
        public Task<List<Movie>> GetRecommended();
    }

    /// <summary>
    /// Users Repository Data.
    /// </summary>
    public class MoviesRepository : RepositoryBase<Movie>, IMoviesRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersRepository"/> class.
        /// </summary>
        /// <param name="moviesDbContext">The database context.</param>
        public MoviesRepository(PolluxMoviesDbContext moviesDbContext)
            : base(moviesDbContext)
        {
        }

        /// <summary>
        /// Gets all by default parameters.
        /// </summary>
        /// <returns>List Movie. </returns>
        public new Task<List<Movie>> GetAll()
        {
            return this.dbSet.Include(p => p.Director)
                .Where(p => p.ProcessedByAzureJob && !p.IsDeleted).ToListAsync();
        }

        /// <summary>
        /// Gets the recommended.
        /// </summary>
        /// <returns></returns>
        public Task<List<Movie>> GetRecommended()
        {
            return this.dbSet.Include(p => p.Director)
                .Where(p => p.ProcessedByAzureJob && !p.IsDeleted)
                .OrderBy(p => p.Likes)
                .Take(15)
                .ToListAsync();
        }

        /// <summary>
        /// Searches the specified search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns>Movie List Search Result.</returns>
        public Task<List<Movie>> Search(string search)
        {
            return this.dbSet.Include(p => p.Director)
               .Where(p => p.ProcessedByAzureJob &&
               !p.IsDeleted && (
                    p.Director.Name.Contains(search) ||
                    p.Name.Contains(search) ||
                    p.Language.Contains(search)))
               .ToListAsync();
        }
    }
}
