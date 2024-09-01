using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Persistence.Repositories.Base;
using Movies.Persistence.Repositories.Base.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Movies.Persistence.Queries;
using System.Linq;
using Movies.Common.Models;

namespace Movies.Persistence.Repositories
{
    /// <summary>
    /// Movies Featured Repository contract.
    /// </summary>
    public interface IMoviesFeaturedRepository : IRepository<MovieFeatured>
    {
        new Task<List<MovieFeaturedQuery>> GetAll();
    }

    /// <summary>
    /// Users Repository Data.
    /// </summary>
    public class MoviesFeaturedRepository : RepositoryBase<MovieFeatured>, IMoviesFeaturedRepository
    {
        private readonly PolluxMoviesDbContext db;


        /// <summary>
        /// Initializes a new instance of the <see cref="MoviesFeaturedRepository"/> class.
        /// </summary>
        /// <param name="moviesDbContext">The database context.</param>
        public MoviesFeaturedRepository(PolluxMoviesDbContext moviesDbContext)
            : base(moviesDbContext)
        {
            this.db = moviesDbContext;
        }

        /// <summary>
        /// Gets all by default parameters.
        /// </summary>
        /// <returns>List Movie. </returns>
        public new async Task<List<MovieFeaturedQuery>> GetAll()
        {
            var sqlQuery =
                from moviesFeatured in this.db.MoviesFeatured
                join movie in this.db.Movies on moviesFeatured.MovieId equals movie.Id
                join genre in this.db.Genres on movie.GenreId equals genre.Id
                join director in this.db.Directors on movie.DirectorId equals director.Id
                select new MovieFeaturedQuery()
                {
                    Genre = genre.Name,
                    CategoryGenres = from movieGenres in this.db.MovieGenres
                        where movieGenres.MovieId == movie.Id
                        select movieGenres.Genre.Name,
                    Movie = movie,
                    Director = director,
                    UrlPortraitImage = moviesFeatured.UrlPortraitImage,
                };


            return await sqlQuery.ToListAsync();
        }
    }
}