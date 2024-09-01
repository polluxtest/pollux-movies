namespace Movies.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using AutoMapper;

    using Movies.Common.Constants.Strings;
    using Movies.Common.Models;
    using Movies.Common.Models.Requests;
    using Movies.Domain.Entities;
    using Movies.Persistence.Queries;
    using Movies.Persistence.Repositories;

    public interface IMoviesListService
    {
        Task AddRemoveAsync(MovieUserRequestGuid model);

        Task DeleteAsync(MovieUserRequestGuid model);

        Task<List<Guid>> GetMoviesIdsByUser(Guid userId);

        Task<List<MoviesByCategoryModel>> GetMovieMyList(string userId);
    }

    public class MoviesListService : IMoviesListService
    {
        private readonly IMoviesListRepository moviesListRepository;
        private readonly IMapper mapper;

        public MoviesListService(
            IMoviesListRepository moviesListRepository,
            IMapper mapper)
        {
            this.moviesListRepository = moviesListRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Adds the remove asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task.</returns>
        public async Task AddRemoveAsync(MovieUserRequestGuid model)
        {
            var exists =
                await this.moviesListRepository.AnyAsync(p => p.MovieId == model.MovieId && p.UserId == model.UserId);

            if (exists)
            {
                await this.DeleteAsync(model);
                return;
            }

            var userMovie = new MoviesLists()
            {
                MovieId = model.MovieId,
                UserId = model.UserId,
                DateAdded = DateTime.UtcNow,
            };

            await this.moviesListRepository.AddAsync(userMovie);
            await this.moviesListRepository.SaveAsync();
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task.</returns>
        public async Task DeleteAsync(MovieUserRequestGuid model)
        {
            var userMovie =
                await this.moviesListRepository.GetAsync(p => p.MovieId == model.MovieId && p.UserId == model.UserId);
            this.moviesListRepository.Delete(userMovie);
            await this.moviesListRepository.SaveAsync();
        }

        /// <summary>
        /// Gets the movie my list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>My List MovieModels</returns>
        public async Task<List<MoviesByCategoryModel>> GetMovieMyList(string userId)
        {
            var myListDb = await this.moviesListRepository.GetMoviesMyList(userId);
            var myList = this.mapper.Map<List<MoviesQuery>, List<MovieModel>>(myListDb);

            return new List<MoviesByCategoryModel>() { new() { Movies = myList, Title = TitleConstants.MyList } };
        }

        /// <summary>
        /// Gets the movies by user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task<List<MoviesLists>.</returns>
        public async Task<List<Guid>> GetMoviesIdsByUser(Guid userId)
        {
            var userMovies = await this.moviesListRepository.GetMoviesListIds(userId);

            return userMovies;
        }
    }
}