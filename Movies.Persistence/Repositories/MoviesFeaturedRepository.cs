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
    /// Movies Featured Repository contract.
    /// </summary>
    public interface IMoviesFeaturedRepository : IRepository<MovieFeatured>
    {
        Task<List<MovieFeatured>> GetAll();
    }

    /// <summary>
    /// Users Repository Data.
    /// </summary>
    public class MoviesFeaturedRepository : RepositoryBase<MovieFeatured>, IMoviesFeaturedRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoviesFeaturedRepository"/> class.
        /// </summary>
        /// <param name="moviesDbContext">The database context.</param>
        public MoviesFeaturedRepository(PolluxMoviesDbContext moviesDbContext)
            : base(moviesDbContext)
        {
        }

        /// <summary>
        /// Gets all by default parameters.
        /// </summary>
        /// <returns>List Movie. </returns>
        public new Task<List<MovieFeatured>> GetAll()
        {
            return this.dbSet
                .Include(p => p.Movie)
                .ThenInclude(p => p.Director)
                .Where(p => p.Movie.ProcessedByAzureJob && !p.Movie.IsDeleted)
                .ToListAsync();
        }
    }
}