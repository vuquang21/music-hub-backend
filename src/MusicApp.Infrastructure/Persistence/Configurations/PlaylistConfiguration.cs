using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicApp.Domain.Entities;

namespace MusicApp.Infrastructure.Persistence.Configurations;

public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
{
    public void Configure(EntityTypeBuilder<Playlist> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.CoverImageUrl).HasMaxLength(500);
        builder.HasOne(p => p.Owner).WithMany(u => u.Playlists)
               .HasForeignKey(p => p.OwnerId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Followers).WithMany(u => u.FollowedPlaylists)
               .UsingEntity(j => j.ToTable("PlaylistFollowers"));
        builder.Ignore(p => p.DomainEvents);
    }
}
