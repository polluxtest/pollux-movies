using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Domain.Entities;

namespace Movies.Persistence.Configurations
{
    public class MoviesLikesConfiguration : IEntityTypeConfiguration<MoviesLikes>
    {
        public void Configure(EntityTypeBuilder<MoviesLikes> builder)
        {
            builder.ToTable<MoviesLikes>("MoviesLikes");
            builder.HasKey(p => new { p.UserId, p.MovieId });
            builder.HasIndex(p => new { p.UserId, p.MovieId });
            builder.HasIndex(p => p.MovieId);
            builder.HasIndex(p => p.UserId);
        }
    }
}
