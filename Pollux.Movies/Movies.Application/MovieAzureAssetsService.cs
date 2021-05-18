namespace Movies.Application
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Movies.Domain.Entities;
    using Pollux.Persistence.Repositories;

    public interface IMovieAzureAssetsService
    {
        Task<List<MovieAzureAsset>> GetNotTransformedMovies();
    }

    public class MovieAzureAssetsService : IMovieAzureAssetsService
    {
        private readonly IMovieAzureAssetsRepository movieAssetRepository;

        public MovieAzureAssetsService(IMovieAzureAssetsRepository movieAssetRepository)
        {
            this.movieAssetRepository = movieAssetRepository;
        }

        /// <summary>
        /// Gets the not transformed movies.
        /// </summary>
        /// <returns></returns>
        public async Task<List<MovieAzureAsset>> GetNotTransformedMovies()
        {
            var moviesToTransform = await this.movieAssetRepository
                .GetManyAsync(p => p.ProccesedByAzureJob == false);

            return moviesToTransform;
        }
    }
}
