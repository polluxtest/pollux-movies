namespace Movies.Application
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Movies.Domain.Entities;
    using Pollux.Persistence.Repositories;

    public interface IMovieAzureAssetsService
    {
        Task<List<MovieAzureAsset>> GetNotTransformedMovies();

        Task Create(int movieId, string outputAsset, string inputAsset);
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
        /// <returns>List of MovieAzureAsset.</returns>
        public async Task<List<MovieAzureAsset>> GetNotTransformedMovies()
        {
            var moviesToTransform = await this.movieAssetRepository
                .GetManyAsync(p => p.ProcessedByAzureJob == false);

            return moviesToTransform;
        }

        /// <summary>
        /// Creates the specified movie identifier.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="outputAsset">The output asset.</param>
        /// <param name="inputAsset">The input asset.</param>
        /// <returns>Task.</returns>
        public Task Create(int movieId, string outputAsset, string inputAsset)
        {
            var movieAzureAsset = new MovieAzureAsset()
            {
                MovieId = movieId,
                AssetOutput = outputAsset,
                AssetInputName = inputAsset,
                ProcessedByAzureJob = true,
            };

            this.movieAssetRepository.Add(movieAzureAsset);
            this.movieAssetRepository.Save();

            return Task.CompletedTask;
        }
    }
}
