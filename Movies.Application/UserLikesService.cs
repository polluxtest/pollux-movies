using AutoMapper;
using Movies.Application.Models;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movies.Application
{

    public interface IUserLikesService
    {
        Task AddRemoveUserLike(AddRemoveUserMovieModel model);
        Task DeleteAsync(AddRemoveUserMovieModel model);
        Task<List<Guid>> GetLikesMoviesIds(string userId);
        public Task<bool> IsMovieLikedByUser(Guid movieId, string userId);
    }

    public class UserLikesService : IUserLikesService
    {
        private readonly IMoviesRepository moviesRepository;
        private readonly IUserLikesRepository userLikesRepository;
        private readonly IMapper mapper;

        public UserLikesService(
            IMoviesRepository moviesRepository,
            IUserLikesRepository userLikesRepository,
            IMapper mapper)
        {
            this.moviesRepository = moviesRepository;
            this.userLikesRepository = userLikesRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Adds the remove user like.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task.</returns>
        public async Task AddRemoveUserLike(AddRemoveUserMovieModel model)
        {
            var exists = await this.userLikesRepository.AnyAsync(p => p.MovieId == model.MovieId && p.UserId == model.UserId);
            var movieDb = await this.moviesRepository.GetAsync(p => p.Id == model.MovieId);

            if (exists)
            {
                await this.UnlikeMovie(model, movieDb);
                return;
            }

            await this.LikeMovie(model, movieDb);
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task.</returns>
        public async Task DeleteAsync(AddRemoveUserMovieModel model)
        {
            var userMovie = await this.userLikesRepository.GetAsync(p => p.MovieId == model.MovieId && p.UserId == model.UserId);
            this.userLikesRepository.Delete(userMovie);
            await this.userLikesRepository.SaveAsync();
        }

        /// <summary>
        /// Gets the likes movies ids.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List liked movies.</returns>
        public Task<List<Guid>> GetLikesMoviesIds(string userId)
        {
            return this.userLikesRepository.GetLikesMoviesIds(userId);
        }


        /// <summary>
        /// Determines whether [is movie liked by user] [the specified movie identifier].
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>True/False.</returns>
        public Task<bool> IsMovieLikedByUser(Guid movieId, string userId)
        {
            return this.userLikesRepository.AnyAsync(p => p.MovieId == movieId && p.UserId.ToString().Equals(userId));
        }

        /// <summary>
        /// Unlikes the movie.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="movieDb">The movie database.</param>
        private async Task UnlikeMovie(AddRemoveUserMovieModel model, Movie movieDb)
        {
            await this.DeleteAsync(model);
            movieDb.Likes -= 1;
            this.UpdateMovie(movieDb);
        }

        /// <summary>
        /// Likes the movie.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="movieDb">The movie database.</param>
        private async Task LikeMovie(AddRemoveUserMovieModel model, Movie movieDb)
        {
            var userMovie = new UserLikes();
            this.mapper.Map(model, userMovie);

            movieDb.Likes += 1;

            await this.userLikesRepository.AddASync(userMovie);
            await this.userLikesRepository.SaveAsync();
            this.UpdateMovie(movieDb);
        }

        /// <summary>
        /// Updates the movie.
        /// </summary>
        /// <param name="movie">The movie.</param>
        private void UpdateMovie(Movie movie)
        {
            this.moviesRepository.Update(movie);
            this.moviesRepository.Save(); // todo race condition on dispose with save async
        }
    }
}
