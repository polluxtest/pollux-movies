using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Domain.Entities;

namespace Movies.Persistence.Configurations
{
    public class MoviesWatchingConfiguration : IEntityTypeConfiguration<MovieWatching>
    {
        public void Configure(EntityTypeBuilder<MovieWatching> builder)
        {
            builder.HasKey(p => new { p.UserId, p.MovieId });
            builder.Property(p => p.ElapsedTime).HasDefaultValue(0).HasColumnType("decimal");
            builder.Property(p => p.Duration).HasDefaultValue(0).HasColumnType("decimal");
            builder.HasOne<Movie>(p => p.Movie);
            builder.HasIndex(p => new { p.UserId, p.MovieId });
        }
    }
}
