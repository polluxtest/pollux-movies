namespace Movies.Application.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using AutoMapper;

    using Movies.Common.Models;
    using Movies.Persistence.Queries;
    using Movies.Persistence.Repositories;

    public interface IMoviesFeaturedService
    {
        Task<List<MovieFeaturedModel>> GetAll();
    }

    public class MoviesFeaturedService : IMoviesFeaturedService
    {
        private readonly IMoviesByGenresService moviesGenreService;
        private readonly IMoviesFeaturedRepository moviesFeaturedRepository;
        private readonly IMapper mapper;

        public MoviesFeaturedService(
            IMoviesByGenresService moviesGenresService,
            IMoviesFeaturedRepository moviesFeaturedRepository,
            IMapper mapper)
        {
            this.moviesGenreService = moviesGenresService;
            this.moviesFeaturedRepository = moviesFeaturedRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets all featured movies.
        /// </summary>
        /// <returns>List<MovieFeaturedQuery/></returns>
        public async Task<List<MovieFeaturedModel>> GetAll()
        {
            var featuredMovies = await this.moviesFeaturedRepository.GetAll();

            var featureMoviesModelList =
                this.mapper.Map<List<MovieFeaturedQuery>, List<MovieFeaturedModel>>(featuredMovies);

            return featureMoviesModelList;
        }
    }
}
