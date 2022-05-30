using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Domain.Entities;

namespace Movies.Persistence.Configurations
{
    public class UserLikesConfiguration : IEntityTypeConfiguration<UserLikes>
    {
        public void Configure(EntityTypeBuilder<UserLikes> builder)
        {
            builder.HasKey(p => new { p.UserId, p.MovieId });
            builder.HasIndex("UserId", "MovieId");
        }
    }
}
