using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Domain.Entities;

namespace Movies.Persistence.Configurations
{
    public class UserMoviesConfiguration : IEntityTypeConfiguration<UserMovies>
    {
        public void Configure(EntityTypeBuilder<UserMovies> builder)
        {
            builder.HasKey(p => p.UserId);
            builder.HasKey(p => p.MovieId);
            builder.HasOne(p => p.Movie);
            builder.HasIndex("UserId");
        }
    }
}
