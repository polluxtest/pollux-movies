using Movies.Domain.Entities;

namespace Movies.Application
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Movies.Domain;
    using Pollux.Persistence.Repositories;

    public interface IMoviesService
    {
        public Task<List<Movie>> GetAll();
    }

    public class MoviesService : IMoviesService
    {
        private readonly IMoviesRepository moviesRepository;

        public MoviesService(IMoviesRepository moviesRepository)
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
    }
}
