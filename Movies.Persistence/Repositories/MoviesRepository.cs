using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Persistence.Configurations;
using Movies.Persistence.Repositories.Base;
using Movies.Persistence.Repositories.Base.Interfaces;

namespace Movies.Persistence.Repositories
{
    /// <summary>
    /// Users Repository contract.
    /// </summary>
    public interface IMoviesRepository : IRepository<Movie>
    {
        Task<Movie> GetAsync(Guid movieId);
        Task<Movie> GetAsyncByName(string name);
        Task<List<Movie>> GetAllAsync();
        Task<List<Movie>> Search(string search);
        Task<List<Movie>> GetRecommendedAsync();
        Task<List<Movie>> GetRecommendedByPolluxAsync();
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
        public new Task<List<Movie>> GetAllAsync()
        {
            return this.dbSet.Include(p => p.Director)
                .Where(p => p.ProcessedByAzureJob && !p.IsDeleted).ToListAsync();
        }

        /// <summary>
        /// Gets the movie asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>Movie.</returns>
        public new Task<Movie> GetAsync(Guid movieId)
        {
            return this.dbSet
                .Include(p => p.Director)
                .SingleOrDefaultAsync(p => p.Id == movieId && p.ProcessedByAzureJob && !p.IsDeleted);
        }

        /// <summary>
        /// Gets the name of the asynchronous by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Movie.</returns>
        /// <exception cref="System.ArgumentException">Movie not found</exception>
        public async Task<Movie> GetAsyncByName(string name)
        {
            var movieDb = await this.dbSet
                .Include(p => p.Director)
                .SingleOrDefaultAsync(p => p.Name.Trim().Equals(name.Trim()) && p.ProcessedByAzureJob && !p.IsDeleted);

            if (movieDb == null) throw new ArgumentException("Movie not found", name);

            return movieDb;
        }

        /// <summary>
        /// Gets the recommended.
        /// </summary>
        /// <returns>List<Movie/></returns>
        public Task<List<Movie>> GetRecommendedAsync()
        {
            return this.dbSet.Include(p => p.Director)
                .Where(p => p.ProcessedByAzureJob && !p.IsDeleted)
                .OrderBy(p => p.Likes)
                .Take(15)
                .ToListAsync();
        }


        /// <summary>
        /// Gets the recommended by pollux.
        /// </summary>
        /// <returns>Task<List<Movie>></returns>
        public async Task<List<Movie>> GetRecommendedByPolluxAsync()
        {
            return await this.dbSet.Include(p => p.Director)
                .Where(p => p.ProcessedByAzureJob && !p.IsDeleted && p.Recommended)
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
