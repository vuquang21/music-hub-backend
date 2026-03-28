using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Enums;

namespace MusicApp.Infrastructure.Persistence.Configurations;

public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Title).HasMaxLength(200).IsRequired();
        builder.Property(a => a.CoverImageUrl).HasMaxLength(500);
        builder.HasOne(a => a.Artist).WithMany(ar => ar.Albums)
               .HasForeignKey(a => a.ArtistId).OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(a => a.Status != AlbumStatus.Removed);
        builder.Ignore(a => a.DomainEvents);
    }
}
