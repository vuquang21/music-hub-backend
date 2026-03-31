using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicApp.Domain.Entities;

namespace MusicApp.Infrastructure.Persistence.Configurations;

public class PodcastConfiguration : IEntityTypeConfiguration<Podcast>
{
    public void Configure(EntityTypeBuilder<Podcast> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.CoverImageUrl)
            .HasMaxLength(500);

        builder.Property(p => p.HostName)
            .HasMaxLength(200);

        // Partial index: only active podcasts are queried on the hot path
        builder.HasIndex(p => p.IsActive)
            .HasFilter("\"IsActive\" = true");

        builder.HasIndex(p => p.CreatedAt);
    }
}
