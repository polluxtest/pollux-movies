using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.Models;
using Movies.Common.Constants.Strings;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;

namespace Movies.Application
{
    public interface IUserMoviesService
    {
        Task AddRemoveAsync(AddRemoveUserMovieModel model);

        Task DeleteAsync(AddRemoveUserMovieModel model);

        Task<List<int>> GetMoviesIdsByUser(string userId);

        Task<List<MoviesByCategoryModel>> GetMovieMyList(string userId);
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
        /// Gets the movie my list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>My List MovieModels</returns>
        public async Task<List<MoviesByCategoryModel>> GetMovieMyList(string userId)
        {
            var myListDb = await this.userMoviesRepository.GetMoviesMyList(userId);
            var myList = this.mapper.Map<List<Movie>, List<MovieModel>>(myListDb);

            return new List<MoviesByCategoryModel>() { new MoviesByCategoryModel() { Movies = myList, Title = TitleConstants.MyList } };
        }

        /// <summary>
        /// Gets the movies by user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task<List<UserMovies>.</returns>
        public async Task<List<int>> GetMoviesIdsByUser(string userId)
        {
            var userMovies = await this.userMoviesRepository.GetMoviesListIds(userId);

            return userMovies;
        }
    }
}
