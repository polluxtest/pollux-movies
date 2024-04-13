using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.Models;
using Movies.Application.Models.Requests;
using Movies.Common.Constants.Strings;
using Movies.Domain.Entities;
using Movies.Persistence.QueryResults;
using Movies.Persistence.Repositories;
using Pitcher;

namespace Movies.Application
{
    public interface IMoviesWatchingService
    {
        Task Save(MovieWatchingSaveModel movieWatchingModel);
        Task<List<MoviesByCategoryModel>> GetAllMoviesContinueWatchingAsync(string userId);
        Task<List<MovieWatching>> GetAllAsync(string userId);
        Task<MovieWatchingModel> GetAsync(Guid movieId, string userId);
    }

    public class MoviesWatchingService : IMoviesWatchingService
    {
        private readonly IMoviesWatchingRepository moviesWatchingRepository;
        private readonly IMoviesRepository moviesRepository;
        private readonly IMoviesByGenresRepository moviesByGenresRepository;
        private readonly IMapper mapper;

        public MoviesWatchingService(
            IMoviesWatchingRepository moviesWatchingRepository,
            IMoviesRepository moviesRepository,
            IMoviesByGenresRepository moviesByGenresRepository,
            IMapper mapper)
        {
            this.moviesWatchingRepository = moviesWatchingRepository;
            this.moviesRepository = moviesRepository;
            this.moviesByGenresRepository = moviesByGenresRepository;
            this.mapper = mapper;
        }


        /// <summary>Saves the specified movie watching model.</summary>
        /// <param name="movieWatchingModel">The movie watching model.</param>
        /// <returns>MovieWatching</returns>
        public async Task Save(MovieWatchingSaveModel movieWatchingModel)
        {
            var movieWatchingDb = await this.moviesWatchingRepository.GetAsync(p => p.UserId == movieWatchingModel.UserId && p.MovieId == movieWatchingModel.MovieId);

            if (movieWatchingDb != null)
            {

                movieWatchingDb.ElapsedTime = movieWatchingModel.ElapsedTime;
                movieWatchingDb.RemainingTime = (int)Math.Round(TimeSpan
                    .FromSeconds(movieWatchingModel.Duration - movieWatchingModel.ElapsedTime).TotalMinutes);
                this.moviesWatchingRepository.Update(movieWatchingDb);
            }
            else
            {
                var movieWatching = new MovieWatching() {
                    MovieId = movieWatchingModel.MovieId,
                    UserId = movieWatchingModel.UserId,
                    ElapsedTime =movieWatchingModel.ElapsedTime,
                    Duration = movieWatchingModel.Duration,
                };
                await this.moviesWatchingRepository.AddAsync(movieWatching);
            }

            await this.moviesWatchingRepository.SaveAsync();
        }

        /// <summary>Gets all movies watching grouped by Continue Watching asynchronous.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List<MovieWatching/></returns>
        public async Task<List<MoviesByCategoryModel>> GetAllMoviesContinueWatchingAsync(string userId)
        {
            var moviesWatchingDb = (await this.moviesWatchingRepository.GetManyAsync(userId)).ToList();
            var moviesWatchingModel = this.mapper.Map<List<MovieWatching>, List<MovieModel>>(moviesWatchingDb);
            return new List<MoviesByCategoryModel>() { new MoviesByCategoryModel() { Movies = moviesWatchingModel, Title = TitleConstants.ContinueWatching } };
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>MovieInfoModel.</returns>
        public async Task<MovieWatchingModel> GetAsync(Guid movieId, string userId)
        {
            var movieDb = await this.moviesRepository.GetAsync(movieId, userId);

            Throw.When(movieDb == null, new ArgumentException($"movie not found id {movieId}"));

            var genres = await this.moviesByGenresRepository.GetGenresByMovieIdAsync(movieId);
            var movieWatchingModel = this.mapper.Map<MoviesQueryResult, MovieWatchingModel>(movieDb);

            movieWatchingModel.Movie.Genres = genres;

            return movieWatchingModel;
        }


        /// <summary>
        /// Gets all movies from movies watching asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List<Movie></returns>
        public async Task<List<MovieWatching>> GetAllAsync(string userId)
        {
            return await this.moviesWatchingRepository.GetManyAsync(userId);
        }
    }
}
