using AzureUploaderTransformerVideos;
using Movies.Application;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.ThirdParty;
using Movies.Persistence;
using Movies.Persistence.Repositories;
using ReadFilesService;
using Microsoft.Extensions.Logging;

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
            services.AddTransient<IMovieGenresRepository, MovieGenresRepository>();
            services.AddTransient<IMoviesRepository, MoviesRepository>();
            services.AddTransient<IMoviesFeaturedRepository, MoviesFeaturedRepository>();
            services.AddTransient<IUserMoviesRepository, UserMoviesRepository>();
            services.AddTransient<IUserLikesRepository, UserLikesRepository>();
            services.AddTransient<IFileReader, FileReader>();
            services.AddTransient<IFileDbWriter, FileDBWriter>();
            services.AddTransient<IGenresRepository, GenresRepository>();
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
            services.AddTransient<IMovieGenresService, MovieGenresService>();
            services.AddTransient<IUserMoviesService, UserMoviesService>();
            services.AddTransient<IUserLikesService, UserLikesService>();
            services.AddTransient<IGenresService, GenresService>();
            services.AddTransient<IImbdService, ImbdService>();
            services.AddTransient<ITranslationService, TranslationService>();
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<ApplicationLogger>>();
            services.AddSingleton(typeof(ILogger), logger);
        }
    }
}