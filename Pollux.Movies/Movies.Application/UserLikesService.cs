using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.Models;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;

namespace Movies.Application
{

    public interface IUserLikesService
    {
        Task AddRemoveUserLike(AddRemoveUserMovieModel model);
        Task DeleteAsync(AddRemoveUserMovieModel model);
        Task<List<int>> GetLikesMoviesIds(string userId);
    }

    public class UserLikesService : IUserLikesService
    {
        private readonly IMoviesService moviesService;
        private readonly IUserLikesRepository userLikesRepository;
        private readonly IMapper mapper;

        public UserLikesService(
            IMoviesService moviesService,
            IUserLikesRepository userLikesRepository,
            IMapper mapper)
        {
            this.moviesService = moviesService;
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
            var movieDb = await this.moviesService.GetAsync(model.MovieId);

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
            this.userLikesRepository.Save();
        }

        /// <summary>
        /// Gets the likes movies ids.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List liked movies.</returns>
        public Task<List<int>> GetLikesMoviesIds(string userId)
        {
            return this.userLikesRepository.GetLikesMoviesIds(userId);
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
            await this.moviesService.UpdateMovie(movieDb);
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
            await this.moviesService.UpdateMovie(movieDb);
            this.userLikesRepository.Save();
        }
    }
}
