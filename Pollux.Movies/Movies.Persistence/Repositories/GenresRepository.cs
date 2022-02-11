using System.Collections.Generic;
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
    public interface IGenresRepository : IRepository<Genre>
    {
        Task AddManyAsync(List<string> genres);
    }

    /// <summary>
    /// Users Repository Data.
    /// </summary>
    public class GenresRepository : RepositoryBase<Genre>, IGenresRepository
    {
        private readonly IMoviesRepository moviesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenresRepository"/> class.
        /// </summary>
        /// <param name="moviesDbContext">The movies database context.</param>
        /// <param name="moviesRepository">The movies repository.</param>
        public GenresRepository(
            PolluxMoviesDbContext moviesDbContext,
            IMoviesRepository moviesRepository)
            : base(moviesDbContext)
        {
            this.moviesRepository = moviesRepository;
        }

        /// <summary>
        /// Adds the many asynchronous.
        /// </summary>
        /// <param name="genres">The genres.</param>
        public async Task AddManyAsync(List<string> genres)
        {
            foreach (var genre in genres)
            {
                var genreNameTrimmed = genre.Trim();

                var genreDb = await this.dbSet.SingleOrDefaultAsync(p => p.Name.Trim().Equals(genreNameTrimmed));
                if (genreDb == null)
                {
                    var newGenreDb = new Genre() { Name = genreNameTrimmed };
                    this.Add(newGenreDb);
                    this.Save();
                }
            }

            this.Save();
        }
    }
}
