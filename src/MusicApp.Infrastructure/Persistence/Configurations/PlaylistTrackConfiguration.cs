using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicApp.Domain.Entities;

namespace MusicApp.Infrastructure.Persistence.Configurations;

public class PlaylistTrackConfiguration : IEntityTypeConfiguration<PlaylistTrack>
{
    public void Configure(EntityTypeBuilder<PlaylistTrack> builder)
    {
        builder.HasKey(pt => new { pt.PlaylistId, pt.TrackId });
        builder.HasOne(pt => pt.Playlist).WithMany(p => p.PlaylistTracks)
               .HasForeignKey(pt => pt.PlaylistId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(pt => pt.Track).WithMany()
               .HasForeignKey(pt => pt.TrackId).OnDelete(DeleteBehavior.Cascade);
    }
}
