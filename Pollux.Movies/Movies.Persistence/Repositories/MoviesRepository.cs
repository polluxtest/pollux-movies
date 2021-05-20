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
    }
}
