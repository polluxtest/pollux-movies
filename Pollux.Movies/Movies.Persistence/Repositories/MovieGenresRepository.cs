using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movies.Persistence.Repositories.Base.Interfaces;
using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories.Base;

namespace Movies.Persistence.Repositories
{
    public interface IMovieGenresRepository : IRepository<MovieGenres>
    {
        Task<List<MovieGenres>> GetAllAsync();
    }

    public class MovieGenresRepository : RepositoryBase<MovieGenres>, IMovieGenresRepository
    {
        public MovieGenresRepository(PolluxMoviesDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <returns>
        /// Entity List.
        /// </returns>
        public new Task<List<MovieGenres>> GetAllAsync()
        {
            return this.dbSet
                .Include(p => p.Genre)
                .Include(p => p.Movie)
                .ThenInclude(p => p.Director)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }
    }
}
