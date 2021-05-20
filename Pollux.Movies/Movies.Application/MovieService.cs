using Movies.Persistence.Repositories;

namespace Movies.Application
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Movies.Domain.Entities;

    public interface IMovieService
    {
        public Task<List<Movie>> GetAll();

        Task UpdateMovie(Movie movie);
    }

    public class MovieService : IMovieService
    {
        private readonly IMoviesRepository moviesRepository;

        public MovieService(IMoviesRepository moviesRepository)
        {
            this.moviesRepository = moviesRepository;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>Movie List.</returns>
        public Task<List<Movie>> GetAll()
        {
            var movies = this.moviesRepository.GetAllAsync(); // todo a soft deleted would be great
            return movies;
        }

        /// <summary>
        /// Updates the specified movie.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns>Task.</returns>
        public Task UpdateMovie(Movie movie)
        {
            this.moviesRepository.Update(movie);
            this.moviesRepository.Save();

            return Task.CompletedTask;
        }
    }
}
