using AzureUploaderTransformerVideos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Movies.Application.Services;
using Movies.Application.ThirdParty;
using Movies.Persistence;
using Movies.Persistence.Repositories;
using ReadFilesService;

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
            services.AddTransient<PolluxMoviesDbContext, PolluxMoviesDbContext>();
            services.AddTransient<IMoviesByGenresRepository, MovieByGenresRepository>();
            services.AddTransient<IMoviesRepository, MoviesRepository>();
            services.AddTransient<IMoviesFeaturedRepository, MoviesFeaturedRepository>();
            services.AddTransient<IMoviesListRepository, MoviesListRepository>();
            services.AddTransient<IUserLikesRepository, MoviesLikesRepository>();
            services.AddTransient<IFileReader, FileReader>();
            services.AddTransient<IFileDbWriter, FileDBWriter>();
            services.AddTransient<IMoviesGenresRepository, MoviesGenresRepository>();
            services.AddTransient<IMoviesWatchingRepository, MoviesWatchingRepository>();
        }

        /// <summary>
        /// Adds the di services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void AddDIServices(this IServiceCollection services)
        {
            services.AddTransient<AzureMediaService, AzureMediaService>();
            services.AddTransient<IMoviesService, MoviesService>();
            services.AddTransient<IMoviesFeaturedService, MoviesFeaturedService>();
            services.AddTransient<IMoviesByGenresService, MoviesByGenresService>();
            services.AddTransient<IMoviesListService, MoviesListService>();
            services.AddTransient<IMoviesLikesService, MoviesLikesService>();
            services.AddTransient<IMoviesGenresService, MoviesGenresService>();
            services.AddTransient<IMoviesWatchingService, MoviesWatchingService>();
            services.AddTransient<IImbdService, ImbdService>();
            services.AddTransient<ITranslationService, TranslationService>();
            services.AddTransient<IMoviesServiceAzure, MoviesServiceAzure>();
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<ApplicationLogger>>();
            services.AddSingleton(typeof(ILogger), logger);
        }
    }
}