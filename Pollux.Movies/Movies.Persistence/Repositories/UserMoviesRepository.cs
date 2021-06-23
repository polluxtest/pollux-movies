using Movies.Domain.Entities;
using Movies.Persistence.Repositories.Base;
using Movies.Persistence.Repositories.Base.Interfaces;

namespace Movies.Persistence.Repositories
{
    public interface IUserMoviesRepository : IRepository<UserMovies>
    {
    }

    public class UserMoviesRepository : RepositoryBase<UserMovies>, IUserMoviesRepository
    {
        public UserMoviesRepository(PolluxMoviesDbContext moviesDbContext)
           : base(moviesDbContext)
        {
        }
    }
}
