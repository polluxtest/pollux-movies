using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Domain.Entities;

namespace Movies.Persistence.Configurations
{
    public class MovieGenresConfiguration : IEntityTypeConfiguration<MovieGenres>
    {
        public void Configure(EntityTypeBuilder<MovieGenres> builder)
        {
            builder.HasKey(p => new { p.MovieId, p.GenreId });
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);
            builder.HasIndex(p => new { p.MovieId, p.GenreId });
            builder.HasIndex(p => p.MovieId);
            builder.HasIndex(p => p.GenreId);
        }
    }
}
