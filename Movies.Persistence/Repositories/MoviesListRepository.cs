using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories.Base;
using Movies.Persistence.Repositories.Base.Interfaces;
using Microsoft.EntityFrameworkCore;
using Movies.Persistence.Queries;

namespace Movies.Persistence.Repositories
{
    public interface IMoviesListRepository : IRepository<MoviesLists>
    {
        Task<List<Guid>> GetMoviesListIds(Guid userId);
        Task<List<MoviesQuery>> GetMoviesMyList(string userId);
    }

    public class MoviesListRepository : RepositoryBase<MoviesLists>, IMoviesListRepository
    {
        private readonly PolluxMoviesDbContext db;

        public MoviesListRepository(PolluxMoviesDbContext moviesDbContext)
           : base(moviesDbContext)
        {
            this.db = moviesDbContext;
        }

        /// <summary>
        /// Gets the movies list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Movie Ids List.</returns>
        public async Task<List<Guid>> GetMoviesListIds(Guid userId)
        {
            return await this.dbSet.Where(p => p.UserId.Equals(userId))
                .Select(p => p.MovieId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the movies my list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List Movie.</returns>
        public async Task<List<MoviesQuery>> GetMoviesMyList(string userId)
        {
            var sqlQuery =
                from movieList in this.db.UserMovies
                join movie in this.db.Movies on movieList.MovieId equals movie.Id
                join genre in this.db.Genres on movie.GenreId equals genre.Id
                join director in this.db.Directors on movie.DirectorId equals director.Id
                join movieWatching in this.db.MoviesWatching on movie.Id equals movieWatching.MovieId into mg
                from moviesWatching in mg.DefaultIfEmpty()
                where moviesWatching.UserId == userId || moviesWatching.UserId == null
                orderby movieList.DateAdded descending
                select new MoviesQuery()
                {
                    Genre = genre.Name,
                    Movie = movie,
                    Director = director,
                    ElapsedTime = moviesWatching != null ? moviesWatching.ElapsedTime : 0,
                    Duration = moviesWatching != null ? moviesWatching.Duration : 0,
                    RemainingTime = moviesWatching != null ? moviesWatching.RemainingTime : 0,
                };

            return await sqlQuery.ToListAsync();
        }
    }
}
