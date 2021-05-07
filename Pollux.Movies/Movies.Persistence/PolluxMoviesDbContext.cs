using Movies.Domain;

namespace Pollux.Persistence
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Core Domain Db Context.
    /// </summary>
    public class PolluxMoviesDbContext : IdentityDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolluxMoviesDbContext"/> class.
        /// </summary>
        /// <param name="options">The options<see cref="DbContextOptions{PolluxDbContext}"/>.</param>
        public PolluxMoviesDbContext(DbContextOptions<PolluxMoviesDbContext> options)
          : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Gets or sets the Users.
        /// </summary>
        public  DbSet<Movie> Movies { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyPersistence.Assembly);
        }
        
    }
}
