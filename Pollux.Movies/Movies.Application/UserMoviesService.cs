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
        Task AddRemoveAsync(AddRemoveUserMovieModel model);

        Task DeleteAsync(AddRemoveUserMovieModel model);

        Task<List<int>> GetMoviesByUser(string userId);
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
        /// Adds the remove asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task.</returns>
        public async Task AddRemoveAsync(AddRemoveUserMovieModel model)
        {
            var exists = await this.userMoviesRepository.AnyAsync(p => p.MovieId == model.MovieId && p.UserId == model.UserId);

            if (exists)
            {
                await this.DeleteAsync(model);
                return;
            }

            var userMovie = new UserMovies();
            this.mapper.Map(model, userMovie);

            await this.userMoviesRepository.AddASync(userMovie);
            this.userMoviesRepository.Save();
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task.</returns>
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
        /// <returns>Task<List<UserMovies>.</returns>
        public async Task<List<int>> GetMoviesByUser(string userId)
        {
            var userMovies = await this.userMoviesRepository.GetMoviesList(userId);

            return userMovies;
        }
    }
}
