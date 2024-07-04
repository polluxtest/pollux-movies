using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    public interface IMoviesGenresRepository : IRepository<Genre>
    {
        Task AddManyAsync(List<string> genres);
    }

    /// <summary>
    /// Users Repository Data.
    /// </summary>
    public class MoviesGenresRepository : RepositoryBase<Genre>, IMoviesGenresRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoviesGenresRepository"/> class.
        /// </summary>
        /// <param name="moviesDbContext">The movies database context.</param>
        /// <param name="moviesRepository">The movies repository.</param>
        public MoviesGenresRepository(PolluxMoviesDbContext moviesDbContext)
            : base(moviesDbContext)
        {
        }

        /// <summary>
        /// Adds the many asynchronous.
        /// </summary>
        /// <param name="genres">The genres.</param>
        /// <returns>Task</returns>
        public async Task AddManyAsync(List<string> genres)
        {
            foreach (var genre in genres)
            {
                var genreNameTrimmed = genre.Trim();

                var genreDb = await this.dbSet.SingleOrDefaultAsync(p => p.Name.Trim().Equals(genreNameTrimmed));
                if (genreDb == null)
                {
                    var newGenreDb = new Genre() { Name = genreNameTrimmed };
                    await this.AddAsync(newGenreDb);
                }
            }

            await this.SaveAsync();
        }
    }
}
