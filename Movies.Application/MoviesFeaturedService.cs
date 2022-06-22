namespace Movies.Application
{
    using AutoMapper;
    using Movies.Application.Models;
    using Movies.Domain.Entities;
    using Movies.Persistence.Repositories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMoviesFeaturedService
    {
        Task<List<MovieFeaturedModel>> GetAll();
    }

    public class MoviesFeaturedService : IMoviesFeaturedService
    {
        private readonly IMovieGenresService moviesGenreService;
        private readonly IMoviesFeaturedRepository moviesFeaturedRepository;
        private IMapper mapper;

        public MoviesFeaturedService(
            IMovieGenresService moviesGenresService,
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
        /// <returns>List<MovieFeaturedModel/></returns>
        public async Task<List<MovieFeaturedModel>> GetAll()
        {
            var featuredMovies = await this.moviesFeaturedRepository.GetAll();
            var featureMoviesModelList = this.mapper.Map<List<MovieFeatured>, List<MovieFeaturedModel>>(featuredMovies);

            // todo this should be added as navigation property
            foreach (var featuredMovie in featureMoviesModelList)
            {
                featuredMovie.Movie.Genres = await this.moviesGenreService.GetAllByMovieIdAsync(featuredMovie.Movie.Id);
            }

            return featureMoviesModelList;
        }
    }
}
