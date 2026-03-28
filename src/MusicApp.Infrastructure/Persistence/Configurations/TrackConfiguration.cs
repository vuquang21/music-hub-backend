using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Enums;

namespace MusicApp.Infrastructure.Persistence.Configurations;

public class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
        builder.OwnsOne(t => t.Isrc, isrc =>
            isrc.Property(i => i.Value).HasColumnName("Isrc").HasMaxLength(15));
        builder.OwnsOne(t => t.Duration, d =>
            d.Property(x => x.Seconds).HasColumnName("DurationSeconds"));
        builder.OwnsOne(t => t.Quality, q =>
        {
            q.Property(x => x.BitRate).HasColumnName("BitRate");
            q.Property(x => x.Format).HasColumnName("AudioFormat").HasMaxLength(10);
        });
        builder.Property(t => t.StorageKey).HasMaxLength(500);
        builder.Property(t => t.CdnUrl).HasMaxLength(500);
        builder.HasOne(t => t.Artist).WithMany(a => a.Tracks)
               .HasForeignKey(t => t.ArtistId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Album).WithMany(a => a.Tracks)
               .HasForeignKey(t => t.AlbumId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(t => t.Genres).WithMany(g => g.Tracks)
               .UsingEntity(j => j.ToTable("TrackGenres"));
        builder.HasQueryFilter(t => t.Status != TrackStatus.Removed);
        builder.Ignore(t => t.DomainEvents);
    }
}
