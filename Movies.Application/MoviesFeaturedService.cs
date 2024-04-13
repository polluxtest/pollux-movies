using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.Models;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;

namespace Movies.Application
{
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
        /// <returns>List<MovieFeaturedModel/></returns>
        public async Task<List<MovieFeaturedModel>> GetAll()
        {
            var featuredMovies = await this.moviesFeaturedRepository.GetAll();
            var featureMoviesModelList = this.mapper.Map<List<MovieFeatured>, List<MovieFeaturedModel>>(featuredMovies);

            // todo possible optimization by querying movies genres
            foreach (var featuredMovie in featureMoviesModelList)
            {
                featuredMovie.Movie.Genres = await this.moviesGenreService.GetAllByMovieIdAsync(featuredMovie.Movie.Id);
            }

            return featureMoviesModelList;
        }
    }
}
