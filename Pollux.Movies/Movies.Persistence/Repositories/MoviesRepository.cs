using Movies.Domain;

namespace Pollux.Persistence.Repositories
{
    

    /// <summary>
    /// Users Repository contract.
    /// </summary>
    public interface IUsersRepository : IRepository<Movie>
    {
    }

    /// <summary>
    /// Users Repository Data.
    /// </summary>
    public class UsersRepository : RepositoryBase<Movie>, IUsersRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersRepository"/> class.
        /// </summary>
        /// <param name="moviesDbContext">The database context.</param>
        public UsersRepository(PolluxMoviesDbContext moviesDbContext)
            : base(moviesDbContext)
        {
        }
    }
}
