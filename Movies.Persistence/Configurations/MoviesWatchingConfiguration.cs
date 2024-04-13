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
            builder.Property(p => p.ElapsedTime).HasDefaultValue(0);
            builder.Property(p => p.Duration).HasDefaultValue(0);
            builder.Property(p => p.RemainingTime).HasDefaultValue(0);
            builder.HasOne(p => p.Movie);
            builder.HasIndex(p => p.UserId)
                .IncludeProperties(p => new { p.Duration, p.ElapsedTime, p.RemainingTime });

            builder.HasIndex(p => p.MovieId);
            builder.HasIndex(p => new { p.UserId, p.MovieId });
        }
    }
}
