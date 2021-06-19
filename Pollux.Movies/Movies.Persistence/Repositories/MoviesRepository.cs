using System.Collections.Generic;
using System.Linq;
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
        public Task<List<Movie>> GetAllByDirector();
        public Task<List<Movie>> GetByDirector(string director);
        public Task<List<Movie>> GetAll();
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
        /// Gets all by director.
        /// </summary>
        /// <returns>List of Movies with director.</returns>
        public async Task<List<Movie>> GetAllByDirector()
        {
            return await this.dbSet.Include(p => p.Director)
                .Where(p => p.IsDeleted == false).ToListAsync();
        }

        /// <summary>
        /// Gets all by director.
        /// </summary>
        /// <param name="director">The director.</param>
        /// <returns>List of movies of a single director.</returns>
        public async Task<List<Movie>> GetByDirector(string director)
        {
            // group by
            return await this.dbSet.Include(p => p.Director)
                .Where(p => p.IsDeleted == false && p.Director.Name == director).ToListAsync();
        }

        /// <summary>
        /// Gets all by default parameters.
        /// </summary>
        /// <returns>List Movie. </returns>
        public new Task<List<Movie>> GetAll()
        {
            return this.dbSet.Where(p => p.ProcessedByAzureJob && !p.IsDeleted).ToListAsync();
        }
    }
}
