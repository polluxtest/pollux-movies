namespace Pollux.Persistence.Repositories
{
    using Movies.Domain.Entities;
    using Pollux.Persistence;

    public interface IMovieAzureAssetsRepository : IRepository<MovieAzureAsset>
    {
    }

    public class MovieAzureAssetsRepository : RepositoryBase<MovieAzureAsset>, IMovieAzureAssetsRepository
    {
        public MovieAzureAssetsRepository(PolluxMoviesDbContext moviesDbContext)
            : base(moviesDbContext)
        {
        }
    }
}
