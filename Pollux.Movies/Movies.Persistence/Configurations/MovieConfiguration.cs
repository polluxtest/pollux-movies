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
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(1000);
            builder.Property(p => p.Gender).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Type).IsRequired().HasMaxLength(100);
            builder.Property(p => p.UrlVideo).HasMaxLength(1000);
            builder.Property(p => p.Subtitles).HasMaxLength(1000);
            builder.Property(p => p.UrlImage).HasMaxLength(1000);
            builder.Property(p => p.Year).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Language).IsRequired().HasMaxLength(100);
            builder.Property(p => p.DirectorId).IsRequired().HasDefaultValue(1);
            builder.Property(p => p.Likes).HasDefaultValue(0);
            builder.Property(p => p.Recommended).HasDefaultValue(false);

            builder.HasOne<Director>(p => p.Director);
            builder.HasIndex("Name", "Gender", "Language", "Likes", "Recommended");
        }
    }
}
