using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Domain.Entities;

namespace Movies.Persistence.Configurations
{
    public class UserMoviesConfiguration : IEntityTypeConfiguration<UserMovies>
    {
        public void Configure(EntityTypeBuilder<UserMovies> builder)
        {
            builder.HasKey(p => new { p.UserId, p.MovieId });
            builder.HasIndex("UserId", "MovieId");
        }
    }
}
