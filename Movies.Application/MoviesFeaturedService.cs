using System;
using System.Collections.Generic;
using System.Text;
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
        private readonly IMoviesFeaturedRepository moviesFeaturedRepository;
        private IMapper mapper;

        public MoviesFeaturedService(
            IMoviesFeaturedRepository moviesFeaturedRepository,
            IMapper mapper)
        {
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

            return this.mapper.Map<List<MovieFeatured>, List<MovieFeaturedModel>>(featuredMovies);
        }
    }
}
