namespace Movies.Application
{
    using AutoMapper;
    using Movies.Application.Models;
    using Movies.Application.Models.Requests;
    using Movies.Common.Constants.Strings;
    using Movies.Domain.Entities;
    using Movies.Persistence.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMoviesWatchingService
    {
        Task<MovieWatchingModel> GetAsync(MovieContinueWatchingRequest request);
        Task Save(MovieWatchingModel movieWatchingModel);
        Task<List<MoviesByCategoryModel>> GetAllAsync(string userId);
    }

    public class MoviesWatchingService : IMoviesWatchingService
    {
        private readonly IMoviesWatchingRepository moviesWatchingRepository;
        private readonly IMapper mapper;

        public MoviesWatchingService(IMoviesWatchingRepository moviesWatchingRepository, IMapper mapper)
        {
            this.moviesWatchingRepository = moviesWatchingRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>MovieWatchingModel</returns>
        public async Task<MovieWatchingModel> GetAsync(MovieContinueWatchingRequest request)
        {
            var movieContinueWatchingDb = await this.moviesWatchingRepository.GetAsync(request.UserId,request.MovieId);
            return this.mapper.Map<MovieWatching, MovieWatchingModel>(movieContinueWatchingDb);
        }

        /// <summary>Saves the specified movie watching model.</summary>
        /// <param name="movieWatchingModel">The movie watching model.</param>
        /// <returns>MovieWatching</returns>
        public async Task Save(MovieWatchingModel movieWatchingModel)
        {
            var movieWatchingDb = await this.moviesWatchingRepository.GetAsync(p => p.UserId == movieWatchingModel.UserId && p.MovieId == movieWatchingModel.MovieId);

            if (movieWatchingDb != null)
            {

                movieWatchingDb.ElapsedTime = movieWatchingModel.ElapsedTime;
                this.moviesWatchingRepository.Update(movieWatchingDb);
            }
            else
            {
                var movieWatching = this.mapper.Map<MovieWatchingModel, MovieWatching>(movieWatchingModel);
                await this.moviesWatchingRepository.AddASync(movieWatching);
            }

            await this.moviesWatchingRepository.SaveAsync();
        }

        /// <summary>Gets all asynchronous.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List<MovieWatching/></returns>
        public async Task<List<MoviesByCategoryModel>> GetAllAsync(string userId)
        {
            var moviesWatchingDb = await this.moviesWatchingRepository.GetManyAsync(userId);
            var moviesWatchingModel = this.mapper.Map<List<Movie>, List<MovieModel>>(moviesWatchingDb);
            return new List<MoviesByCategoryModel>() { new MoviesByCategoryModel() { Movies = moviesWatchingModel, Title = TitleConstants.ContinueWatching } };
        }
    }
}
