using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.Models;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;

namespace Movies.Application
{
    public interface IUserMoviesService
    {
        Task AddAsync(AddRemoveUserMovieModel model);

        Task DeleteAsync(AddRemoveUserMovieModel model);

        Task<List<UserMovies>> GetMoviesByUser(Guid userId);
    }

    public class UserMoviesService : IUserMoviesService
    {
        private readonly IUserMoviesRepository userMoviesRepository;
        private readonly IMapper mapper;

        public UserMoviesService(
            IUserMoviesRepository userMoviesRepository,
            IMapper mapper)
        {
            this.userMoviesRepository = userMoviesRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        public async Task AddAsync(AddRemoveUserMovieModel model)
        {
            var userMovie = new UserMovies();
            this.mapper.Map(model, userMovie);

            await this.userMoviesRepository.AddASync(userMovie);
            this.userMoviesRepository.Save();
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        public async Task DeleteAsync(AddRemoveUserMovieModel model)
        {
            var userMovie = await this.userMoviesRepository.GetAsync(p => p.MovieId == model.MovieId && p.UserId == model.UserId);
            this.userMoviesRepository.Delete(userMovie);
            this.userMoviesRepository.Save();
        }

        /// <summary>
        /// Gets the movies by user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task <List<UserMovies>>.</returns>
        public async Task<List<UserMovies>> GetMoviesByUser(Guid userId)
        {
            var userMovies = await this.userMoviesRepository.GetManyAsync(p => p.UserId == userId);

            return userMovies;
        }
    }
}
