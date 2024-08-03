using Movies.Domain;
using Movies.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Movies.Persistence.Configurations
{
    /// <summary>
    /// EF Configuration.
    /// </summary>
    /// <seealso cref="Movie" />
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(p => p.Id).HasAnnotation("SqlServer:Clustered", false);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(1000);
            builder.Property(p => p.DescriptionEs).IsRequired().HasMaxLength(1000);
            builder.Property(p => p.UrlVideo).HasMaxLength(1000);
            builder.Property(p => p.Subtitles).HasMaxLength(1000);
            builder.Property(p => p.UrlImage).HasMaxLength(1000);
            builder.Property(p => p.UrlCoverImage).HasMaxLength(1000);
            builder.Property(p => p.Year).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Language).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Imbd).IsRequired().HasMaxLength(5);
            builder.Property(p => p.DirectorId).IsRequired().HasDefaultValue(1);
            builder.Property(p => p.Likes).HasDefaultValue(0);
            builder.Property(p => p.Recommended).HasDefaultValue(false);
            builder.Property(p => p.ProcessedByStreamVideo).HasDefaultValue(false);
            builder.Property(p => p.GenreId).HasDefaultValue(1);

            builder.HasOne<Director>(p => p.Director);
            builder.HasOne<Genre>(p => p.Genre);
            builder.HasMany<MovieWatching>(p => p.MoviesWatching)
                .WithOne(p => p.Movie);

            builder.HasIndex(p => p.Id);
            builder.HasIndex(p => p.Language);
            builder.HasIndex(p => p.DirectorId);
            builder.HasIndex(p => p.Imbd);
            builder.HasIndex(p => p.Likes);
            builder.HasIndex(p => p.Recommended);
            builder.HasIndex(p => p.Name).IsClustered();
            builder.HasIndex(p => p.GenreId);
            builder.HasIndex("IsDeleted", "ProcessedByStreamVideo")
                .HasFilter("[IsDeleted] = 0 and [ProcessedByStreamVideo] = 1");
        }
    }
}