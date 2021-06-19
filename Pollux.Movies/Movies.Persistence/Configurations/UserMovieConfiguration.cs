using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Domain.Entities;

namespace Movies.Persistence.Configurations
{
    public class UserMovieConfiguration : IEntityTypeConfiguration<UserMovie>
    {
        public void Configure(EntityTypeBuilder<UserMovie> builder)
        {
            builder.HasKey(p => p.UserId);
            builder.HasKey(p => p.MovieId);
            builder.HasOne(p => p.Movie);
            builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.HasIndex("UserId");
        }
    }
}
