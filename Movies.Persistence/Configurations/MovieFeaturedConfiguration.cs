
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Domain;
using Movies.Domain.Entities;

namespace Movies.Persistence.Configurations
{
    public class MovieFeaturedConfiguration : IEntityTypeConfiguration<MovieFeatured>
    {
        public void Configure(EntityTypeBuilder<MovieFeatured> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.HasIndex(p => p.MovieId).IsUnique();
            builder.Property(p => p.UrlPortraitImage).HasMaxLength(1000);
            builder.HasOne<Movie>(p => p.Movie);
        }
    }
}
