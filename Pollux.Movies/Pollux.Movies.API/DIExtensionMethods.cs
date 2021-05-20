using Movies.Application;
using Microsoft.Extensions.DependencyInjection;
using Movies.Persistence;
using Movies.Persistence.Repositories;

namespace Pollux.Movies
{
    public static class DIExtensionMethods
    {
        /// <summary>
        /// Adds the di repositories.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddDIRepositories(this IServiceCollection services)
        {
            services.AddTransient<IMoviesRepository, MoviesRepository>();
        }

        /// <summary>
        /// Adds the di services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void AddDIServices(this IServiceCollection services)
        {
            services.AddTransient<PolluxMoviesDbContext, PolluxMoviesDbContext>();
            services.AddTransient<IMovieService, MovieService>();
        }
    }
}