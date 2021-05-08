namespace Pollux.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Movies.Domain;

    /// <summary>
    /// Core Domain Db Context.
    /// </summary>
    public class PolluxMoviesDbContext : DbContext
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
        public DbSet<Movie> Movies { get; set; }

        /// <summary>
        /// Gets or sets the directos.
        /// </summary>
        /// <value>
        /// The directos.
        /// </value>
        public DbSet<Director> Directors { get; set; }

        /// <summary>
        /// Override this method to further configure the model that was discovered by convention from the entity types
        /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
        /// and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        /// define extension methods on this object that allow you to configure aspects of the model that are specific
        /// to a given database.</param>
        /// <remarks>
        /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        /// then this method will not be run.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyPersistence.Assembly);
        }
    }
}
