using Microsoft.Extensions.DependencyInjection;

namespace Pollux.Movies
{
    public static class DIExtensionMethods
    {
        /// <summary>
        /// Adds the di repositories as an extension methods for the startup .
        /// <param name="services">The service collection.</param>
        /// </summary>
        public static void AddDIRepositories(this IServiceCollection services)
        {
            //services.AddScoped<IUsersRepository, UsersRepository>();
        }

        /// <summary>
        /// Adds the di services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void AddDIServices(this IServiceCollection services)
        {
            //services.AddScoped<DbContext, PolluxDbContext>();
        }

        /// <summary>
        /// Adds the identity server services.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddDIIdentityServerServices(this IServiceCollection services)
        {

        }
    }
}