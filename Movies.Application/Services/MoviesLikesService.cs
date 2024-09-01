namespace Movies.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using AutoMapper;

    using Movies.Common.Models.Requests;
    using Movies.Domain.Entities;
    using Movies.Persistence.Repositories;

    public interface IMoviesLikesService
    {
        Task AddRemoveUserLike(MovieUserRequestGuid model);

        Task DeleteAsync(MovieUserRequestGuid model);

        Task<List<Guid>> GetLikesMoviesIds(Guid userId);
    }

    public class MoviesLikesService : IMoviesLikesService
    {
        private readonly IMoviesRepository moviesRepository;
        private readonly IUserLikesRepository userLikesRepository;
        private readonly IMapper mapper;

        public MoviesLikesService(
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
        public async Task AddRemoveUserLike(MovieUserRequestGuid model)
        {
            var exists =
                await this.userLikesRepository.AnyAsync(p => p.MovieId == model.MovieId && p.UserId == model.UserId);
            var movieDb = await this.moviesRepository.GetAsync(p => p.Id == model.MovieId);

            if (exists)
            {
                await this.UnlikeMovie(model, movieDb);
                return;
            }

            await this.LikeMovie(model, movieDb);
            await this.userLikesRepository.SaveAsync();
            await this.moviesRepository.SaveAsync();
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task.</returns>
        public async Task DeleteAsync(MovieUserRequestGuid model)
        {
            var userMovie =
                await this.userLikesRepository.GetAsync(p => p.MovieId == model.MovieId && p.UserId == model.UserId);
            this.userLikesRepository.Delete(userMovie);

            await this.userLikesRepository.SaveAsync();
        }

        /// <summary>
        /// Gets the likes movies ids.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List liked movies.</returns>
        public Task<List<Guid>> GetLikesMoviesIds(Guid userId)
        {
            return this.userLikesRepository.GetLikesMoviesIds(userId);
        }

        /// <summary>
        /// Unlikes the movie.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="movieDb">The movie database.</param>
        /// <returns>Task</returns>
        private async Task UnlikeMovie(MovieUserRequestGuid model, Movie movieDb)
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
        /// <returns>Task</returns>
        private async Task LikeMovie(MovieUserRequestGuid model, Movie movieDb)
        {
            var userMovie = new MoviesLikes()
            {
                UserId = model.UserId,
                MovieId = model.MovieId,
            };

            movieDb.Likes += 1;

            this.UpdateMovie(movieDb);
            await this.userLikesRepository.AddAsync(userMovie);
        }

        /// <summary>
        /// Updates the movie.
        /// </summary>
        /// <param name="movie">The movie.</param>
        private void UpdateMovie(Movie movie)
        {
            this.moviesRepository.Update(movie);
        }
    }
}
