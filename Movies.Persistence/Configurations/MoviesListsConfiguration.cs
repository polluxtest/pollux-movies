using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Domain.Entities;

namespace Movies.Persistence.Configurations
{
    public class MoviesListsConfiguration : IEntityTypeConfiguration<MoviesLists>
    {
        public void Configure(EntityTypeBuilder<MoviesLists> builder)
        {
            builder.ToTable<MoviesLists>("MoviesLists");
            builder.HasKey(p => new { p.UserId, p.MovieId });
            builder.HasIndex(p => new { p.UserId, p.MovieId });
            builder.HasIndex(p => p.MovieId);
            builder.HasIndex(p => p.UserId);
        }
    }
}
