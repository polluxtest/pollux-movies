using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;

namespace Movies.Application.ThirdParty
{
    public interface IMoviesServiceAzure
    {
        Task<List<Movie>> GetAllAsync();
        Task<List<Movie>> GetAllMoviesImagesFilter(bool processedByAzureJob = true);
        Task<List<Movie>> GetAllMoviesCoverImagesFilter(bool processedByAzureJob = false);
        Task AddGenresAsync(Dictionary<string, List<string>> movieGenres);
    }

    public class MoviesServiceAzure : IMoviesServiceAzure
    {
        private readonly IMoviesRepository moviesRepository;
        private readonly IMoviesByGenresService moviesByGenresService;
        private readonly IMoviesGenresService moviesGenresService;

        public MoviesServiceAzure(
            IMoviesRepository moviesRepository,
            IMoviesByGenresService moviesByGenresService,
            IMoviesGenresService moviesGenresService)
        {
            this.moviesRepository = moviesRepository;
            this.moviesByGenresService = moviesByGenresService;
            this.moviesGenresService = moviesGenresService;
        }


        /// <summary>
        /// Gets all movies.
        /// </summary>
        /// <returns><List<Movie>></returns>
        public Task<List<Movie>> GetAllAsync()
        {
            var movies = this.moviesRepository.GetAllAsync();
            return movies;
        }


        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="processedByAzureJob">if set to <c>true</c> [processed by azure job].</param>
        /// <returns>Movie List.</returns>
        public Task<List<Movie>> GetAllMoviesImagesFilter(bool processedByAzureJob = true)
        {
            var movies = this.moviesRepository
                .GetManyAsync(p =>
                    p.ProcessedByStreamVideo == processedByAzureJob &&
                    string.IsNullOrEmpty(p.UrlImage));
            return movies;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="processedByAzureJob">if set to <c>true</c> [processed by azure job].</param>
        /// <returns>Movie List.</returns>
        public Task<List<Movie>> GetAllMoviesCoverImagesFilter(bool processedByAzureJob = false)
        {
            var movies = this.moviesRepository
                .GetManyAsync(p =>
                    p.ProcessedByStreamVideo == processedByAzureJob &&
                    string.IsNullOrEmpty(p.UrlCoverImage));
            return movies;
        }

        /// <summary>
        /// Adds the genres asynchronous.
        /// </summary>
        /// <param name="movieGenres">The movie genres.</param>
        /// <exception cref="System.ArgumentException">Genre not found</exception>
        /// <returns>Task</returns>
        public async Task AddGenresAsync(Dictionary<string, List<string>> movieGenres)
        {
            var genresDb = await this.moviesGenresService.GetAllAsync();

            foreach (var (movieName, genres) in movieGenres)
            {
                var movieDb = await this.GetByNameAsync(movieName);
                var genresIDs = new List<int>();

                foreach (var genre in genres)
                {
                    var genreDb = genresDb.SingleOrDefault(p =>
                        p.Name.Trim().Equals(genre.Trim(), StringComparison.CurrentCultureIgnoreCase));

                    if (genreDb == null) throw new ArgumentException("Genre not found", genre);

                    genresIDs.Add(genreDb.Id);
                }

                await this.moviesByGenresService.AddManyToMovieAsync(movieDb.Id, genresIDs);
            }
        }


        /// <summary>
        /// Gets the by name asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Movie</returns>
        public Task<Movie> GetByNameAsync(string name)
        {
            return this.moviesRepository.GetAsyncByName(name);
        }
    }
}
