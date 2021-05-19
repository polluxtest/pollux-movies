using Movies.Application;

namespace Pollux.Movies
{
    using Microsoft.Extensions.DependencyInjection;
    using Persistence;
    using Persistence.Repositories;

    public static class DIExtensionMethods
    {
        /// <summary>
        /// Adds the di repositories.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddDIRepositories(this IServiceCollection services)
        {
            services.AddTransient<IMoviesRepository, MoviesRepository>();
            services.AddTransient<IMovieAzureAssetsRepository, MovieAzureAssetsRepository>();
        }

        /// <summary>
        /// Adds the di services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void AddDIServices(this IServiceCollection services)
        {
            services.AddTransient<PolluxMoviesDbContext, PolluxMoviesDbContext>();
            services.AddTransient<IMovieAzureAssetsService, MovieAzureAssetsService>();
            services.AddTransient<IMovieService, MovieService>();
        }
    }
}