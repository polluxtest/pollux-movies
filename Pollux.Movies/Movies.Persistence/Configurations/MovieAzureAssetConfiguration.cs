namespace Movies.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Movies.Domain.Entities;

    public class MovieAzureAssetConfiguration : IEntityTypeConfiguration<MovieAzureAsset>
    {
        public void Configure(EntityTypeBuilder<MovieAzureAsset> builder)
        {
            builder.HasKey(p => p.MovieId).IsClustered();
            builder.Property(p => p.ProccesedByAzureJob).IsRequired().HasDefaultValue(false);
            builder.Property(p => p.AssetInputName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.AssetOutput).IsRequired().HasMaxLength(100);

            builder.HasOne(p => p.Movie);

            builder.HasIndex("AssetOutput");
        }
    }
}
