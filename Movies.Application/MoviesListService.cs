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
    public interface IMoviesListService
    {
        Task AddRemoveAsync(AddRemoveUserMovieModel model);

        Task DeleteAsync(AddRemoveUserMovieModel model);

        Task<List<Guid>> GetMoviesIdsByUser(string userId);

        Task<List<MoviesByCategoryModel>> GetMovieMyList(string userId);

        Task<bool> IsMovieInListByUser(Guid movieId, string userId);
    }

    public class MoviesListService : IMoviesListService
    {
        private readonly IMoviesListRepository userMoviesRepository;
        private readonly IMapper mapper;

        public MoviesListService(
            IMoviesListRepository userMoviesRepository,
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

            var userMovie = new MoviesLists();
            this.mapper.Map(model, userMovie);

            await this.userMoviesRepository.AddAsync(userMovie);
            await this.userMoviesRepository.SaveAsync();
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
            await this.userMoviesRepository.SaveAsync();
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
        /// Determines whether [is movie liked by user] [the specified movie identifier].
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>True/False.</returns>
        public Task<bool> IsMovieInListByUser(Guid movieId, string userId)
        {
            return this.userMoviesRepository.AnyAsync(p => p.MovieId == movieId && p.UserId.ToString().Equals(userId));
        }

        /// <summary>
        /// Gets the movies by user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task<List<MoviesLists>.</returns>
        public async Task<List<Guid>> GetMoviesIdsByUser(string userId)
        {
            var userMovies = await this.userMoviesRepository.GetMoviesListIds(userId);

            return userMovies;
        }
    }
}
