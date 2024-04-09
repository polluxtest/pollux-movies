using System.Collections.Generic;
using System.Threading.Tasks;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;

namespace Movies.Application
{
    public interface IMoviesGenresService
    {
        Task AddAsync(List<string> genres);
        Task<List<Genre>> GetAllAsync();
    }

    public class MoviesGenresService : IMoviesGenresService
    {
        private readonly IMoviesGenresRepository genresRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoviesGenresService"/> class.
        /// </summary>
        /// <param name="genresRepository">The genres repository.</param>
        public MoviesGenresService(IMoviesGenresRepository genresRepository)
        {
            this.genresRepository = genresRepository;
        }

        /// <summary>
        /// Adds the genres movies asynchronous.
        /// </summary>
        /// <param name="movieGenresList">The movie genres list.</param>
        public async Task AddAsync(List<string> movieGenresList)
        {
            await this.genresRepository.AddManyAsync(movieGenresList);
        }

        /// <summary>
        /// Gets all Genres async.
        /// </summary>
        /// <returns>List<Genre>.</returns>
        public async Task<List<Genre>> GetAllAsync()
        {
            return await this.genresRepository.GetAllAsync();
        }
    }
}