using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using AutoMapper;
using Movies.Application.Models;
using Movies.Common.Constants.Strings;
using Movies.Common.ExtensionMethods;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories;
using Pitcher;
using Movies.Application.ExtensionMethods;

namespace Movies.Application
{
    public interface IMoviesService
    {
        Task<List<Movie>> GetAll(bool processedByAzureJob = false);

        Task<List<Movie>> GetAllMoviesImagesFilter(bool processedByAzureJob = false);

        Task<List<Movie>> GetAllMoviesCoverImagesFilter(bool processedByAzureJob = false);

        Task<List<MoviesByCategoryModel>> GetByLanguage(string sortBy = null);

        Task<List<MoviesByCategoryModel>> GetByDirector(string sortBy = null);

        Task<List<MoviesByCategoryModel>> GetByGenreAsync(string sortBy = null);

        Task UpdateMovie(Movie movie);

        Task Add(Movie movie);

        Task<List<MovieModel>> Search(string search);

        Task<List<MoviesByCategoryModel>> GetRecommendedByUsers();

        Task<List<MoviesByCategoryModel>> GetRecommendedByPollux();

        Task<MovieInfoModel> GetAsync(Guid movieId, string userId);

        Task AddGenresAsync(Dictionary<string, List<string>> movieGenres);

        Task<Movie> GetAsync(Guid movieId);

        Task<List<string>> GetMoviesNames();

    }

    public class MoviesService : IMoviesService
    {
        private readonly IMoviesRepository moviesRepository;
        private readonly IUserMoviesService userMoviesService;
        private readonly IUserLikesService userMovieLikesService;
        private readonly IMapper mapper;
        private readonly IGenresService genresService;
        private readonly IMovieGenresService movieGenresService;

        public MoviesService(
            IMoviesRepository moviesRepository,
            IUserMoviesService userMoviesService,
            IUserLikesService userMovieLikesService,
            IGenresService genresService,
            IMovieGenresService movieGenreService,
            IMapper mapper)
        {
            this.moviesRepository = moviesRepository;
            this.userMoviesService = userMoviesService;
            this.userMovieLikesService = userMovieLikesService;
            this.genresService = genresService;
            this.movieGenresService = movieGenreService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="processedByAzureJob">if set to <c>true</c> [processed by azure job].</param>
        /// <returns>Movie List.</returns>
        public Task<List<Movie>> GetAll(bool processedByAzureJob = true)
        {
            var movies = this.moviesRepository
                .GetManyAsync(p => p.ProcessedByAzureJob == processedByAzureJob && p.IsDeleted == false);
            return movies;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="processedByAzureJob">if set to <c>true</c> [processed by azure job].</param>
        /// <returns>Movie List.</returns>
        public Task<List<Movie>> GetAllMoviesImagesFilter(bool processedByAzureJob = true)
        {
            var movies = this.moviesRepository
                .GetManyAsync(p => p.ProcessedByAzureJob == processedByAzureJob && p.IsDeleted == false && string.IsNullOrEmpty(p.UrlImage));
            return movies;
        }


        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="processedByAzureJob">if set to <c>true</c> [processed by azure job].</param>
        /// <returns>Movie List.</returns>
        public Task<List<Movie>> GetAllMoviesCoverImagesFilter(bool processedByAzureJob = false)
        {
            var movies = this.moviesRepository
                .GetManyAsync(p => p.ProcessedByAzureJob == processedByAzureJob && p.IsDeleted == false && string.IsNullOrEmpty(p.UrlCoverImage));
            return movies;
        }

        /// <summary>
        /// Gets the by language.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>MovieModel List by Language.</returns>
        public async Task<List<MoviesByCategoryModel>> GetByLanguage(string sortBy = null)
        {
            var moviesDb = await this.moviesRepository.GetAll();

            moviesDb = moviesDb.SortCustomBy(sortBy);

            var moviesGroupedByLanguage = moviesDb
                .GroupBy(p => p.Language)
                .Select(p => new MoviesByCategoryModel()
                {
                    Title = $"{p.Key} {TitleConstants.Language}",
                    Movies = this.mapper.Map<List<Movie>, List<MovieModel>>(p.ToList()),
                }).OrderByDescending(y => y.Movies.Count());

            var movies = moviesGroupedByLanguage.ToList();

            return movies;
        }

        /// <summary>
        /// Gets the by director.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>MovieModel List by Director.</returns>
        public async Task<List<MoviesByCategoryModel>> GetByDirector(string sortBy = null)
        {
            var moviesDb = await this.moviesRepository.GetAll();

            moviesDb = moviesDb.SortCustomBy(sortBy);

            var moviesGroupedByDirector = moviesDb
                .GroupBy(p => p.Director.Name)
                .Select(p => new MoviesByCategoryModel()
                {
                    Title = p.Key,
                    Movies = this.mapper.Map<List<Movie>, List<MovieModel>>(p.ToList()),
                }).OrderByDescending(y => y.Movies.Count());

            var movies = moviesGroupedByDirector.ToList();

            return movies;
        }

        /// <summary>
        /// Gets the by genre asynchronous.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>List<MoviesByCategoryModel/></returns>
        public async Task<List<MoviesByCategoryModel>> GetByGenreAsync(string sortBy = null)
        {
            var movies = await this.movieGenresService.GetAllByGenreAsync(sortBy);

            return movies;
        }

        /// <summary>
        /// Sorts the movies.
        /// </summary>
        /// <param name="movies">The movies.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>Movies Sorted.</returns>
        public Task<List<MoviesByCategoryModel>> SortMoviesReels(ref List<MoviesByCategoryModel> movies, string sortBy = null)
        {
            if (string.IsNullOrEmpty(sortBy)) return Task.FromResult(movies);

            switch (sortBy)
            {
                case SortByConstants.AlphaAscending: movies = movies.OrderBy(p => p.Title).ToList(); break;
                case SortByConstants.AlphaDescending: movies = movies.OrderByDescending(p => p.Title).ToList(); break;
            }

            return Task.FromResult(movies);
        }




        /// <summary>
        /// Updates the specified movie.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns>Task.</returns>
        public Task UpdateMovie(Movie movie)
        {
            this.moviesRepository.Update(movie);
            this.moviesRepository.Save();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds the specified movie.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns>Task.</returns>
        public Task Add(Movie movie)
        {
            this.moviesRepository.Add(movie);
            this.moviesRepository.Save();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Searches the specified search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns>List<MovieModel></returns>
        public async Task<List<MovieModel>> Search(string search)
        {
            var moviesDbSearch = await this.moviesRepository.Search(search);
            var movies = this.mapper.Map<List<Movie>, List<MovieModel>>(moviesDbSearch);

            return movies;
        }

        /// <summary>
        /// Gets the recommended by users.
        /// </summary>
        /// <returns>List<MoviesByCategoryModel/>.</returns>
        public async Task<List<MoviesByCategoryModel>> GetRecommendedByPollux()
        {
            var moviesDb = await this.moviesRepository
                .GetManyAsync(p => p.ProcessedByAzureJob == true && p.IsDeleted == false && p.Recommended);

            var movies = this.mapper.Map<List<Movie>, List<MovieModel>>(moviesDb);

            return new List<MoviesByCategoryModel>() { new MoviesByCategoryModel() { Movies = movies, Title = TitleConstants.RecommendedByPollux } };
        }

        /// <summary>
        /// Gets the recommended by users.
        /// </summary>
        /// <returns>List<MoviesByCategoryModel/>.</returns>
        public async Task<List<MoviesByCategoryModel>> GetRecommendedByUsers()
        {
            var moviesDb = await this.moviesRepository.GetRecommended();

            var movies = this.mapper.Map<List<Movie>, List<MovieModel>>(moviesDb);

            return new List<MoviesByCategoryModel>() { new MoviesByCategoryModel() { Movies = movies, Title = TitleConstants.RecommendedByUsers } };

        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>MovieInfoModel.</returns>
        public async Task<MovieInfoModel> GetAsync(Guid movieId, string userId)
        {
            var movieInfoModel = new MovieInfoModel();
            var movieDb = await this.moviesRepository.GetAsync(movieId);
            var genres = await this.movieGenresService.GetAllByMovieIdAsync(movieId);

            Throw.When(movieDb == null, new ArgumentException($"movie not found id {movieId}"));

            movieInfoModel.IsInList = await this.userMoviesService.IsMovieInListByUser(movieId, userId);
            movieInfoModel.IsLiked = await this.userMovieLikesService.IsMovieLikedByUser(movieId, userId);
            movieInfoModel.Genres = genres;

            return this.mapper.Map<Movie, MovieInfoModel>(movieDb, movieInfoModel);
        }

        /// <summary>
        /// Adds the genres.
        /// </summary>
        /// <param name="movieGenres">The movie genres.</param>
        public async Task AddGenresAsync(Dictionary<string, List<string>> movieGenres)
        {
            var genresDb = await this.genresService.GetAllAsync();

            foreach (var (movieName, genres) in movieGenres)
            {
                var movieDb = await this.GetByNameAsync(movieName);
                var genresIDs = new List<int>();

                foreach (var genre in genres)
                {
                    var genreDb = genresDb.SingleOrDefault(p =>
                        p.Name.Trim().Equals(genre.Trim(), StringComparison.CurrentCultureIgnoreCase));

                    if (genreDb == null) throw new ArgumentException("Genre not found", genre);

                    genresIDs.Add(genreDb.Id);
                }

                await this.movieGenresService.AddManyToMovieAsync(movieDb.Id, genresIDs);
            }
        }

        /// <summary>
        /// Gets the by name asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Movie</returns>
        public Task<Movie> GetByNameAsync(string name)
        {
            return this.moviesRepository.GetAsyncByName(name);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>Movie.</returns>
        public Task<Movie> GetAsync(Guid movieId)
        {
            return this.moviesRepository.GetAsync(p => p.Id == movieId);
        }

        /// <summary>
        /// Gets the movies names.
        /// </summary>
        /// <returns>List<string/></returns>
        public async Task<List<string>> GetMoviesNames()
        {
            var moviesList = await this.GetAll();

            return moviesList.Select(p => p.Name).ToList();
        }
    }
}
