namespace Movies.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Movies.Common.Constants.Strings;
    using Movies.Common.Models;
    using Movies.Common.Models.Requests;
    using Movies.Domain.Entities;
    using Movies.Persistence.Queries;
    using Movies.Persistence.Repositories;

    using Pitcher;

    public interface IMoviesWatchingService
    {
        Task Save(MovieWatchingSaveRequest movieWatchingModel);

        Task<List<MoviesByCategoryModel>> GetAllMoviesContinueWatchingAsync(string userId);
    }

    public class MoviesWatchingService : IMoviesWatchingService
    {
        private readonly IMoviesWatchingRepository moviesWatchingRepository;

        private readonly IMoviesByGenresRepository moviesByGenresRepository;

        private readonly IMapper mapper;

        public MoviesWatchingService(
            IMoviesWatchingRepository moviesWatchingRepository,
            IMoviesByGenresRepository moviesByGenresRepository,
            IMapper mapper)
        {
            this.moviesWatchingRepository = moviesWatchingRepository;
            this.moviesByGenresRepository = moviesByGenresRepository;
            this.mapper = mapper;
        }


        /// <summary>Saves the specified movie watching model.</summary>
        /// <param name="movieWatchingModel">The movie watching model.</param>
        /// <returns>MovieWatching</returns>
        public async Task Save(MovieWatchingSaveRequest movieWatchingModel)
        {
            var movieWatchingDb = await this.moviesWatchingRepository.GetAsync(p =>
                p.UserId == movieWatchingModel.UserId && p.MovieId == movieWatchingModel.MovieId);

            if (movieWatchingDb != null)
            {
                movieWatchingDb.ElapsedTime = movieWatchingModel.ElapsedTime;
                movieWatchingDb.RemainingTime = movieWatchingModel.RemainingTime;
                movieWatchingDb.DateAdded = DateTime.UtcNow;
                this.moviesWatchingRepository.Update(movieWatchingDb);
            }
            else
            {
                var movieWatching = new MovieWatching()
                {
                    MovieId = movieWatchingModel.MovieId,
                    UserId = movieWatchingModel.UserId,
                    ElapsedTime = movieWatchingModel.ElapsedTime,
                    Duration = movieWatchingModel.Duration,
                    RemainingTime = movieWatchingModel.RemainingTime,
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

            return new List<MoviesByCategoryModel>()
            {
                new() { Movies = moviesWatchingModel, Title = TitleConstants.ContinueWatching },
            };
        }
    }
}