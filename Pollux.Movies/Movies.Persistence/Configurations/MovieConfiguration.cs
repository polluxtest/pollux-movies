namespace Movies.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Movies.Domain;

    /// <summary>
    /// EF Configuration.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration{Movies.Domain.Movie}" />
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(1000);
            builder.Property(p => p.Gender).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Type).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Url).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Year).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Language).IsRequired().HasMaxLength(100);

            builder.HasIndex("Name", "Gender", "Language");
        }
    }
}
