using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicApp.Domain.Entities;

namespace MusicApp.Infrastructure.Persistence.Configurations;

public class ListeningHistoryConfiguration : IEntityTypeConfiguration<ListeningHistory>
{
    public void Configure(EntityTypeBuilder<ListeningHistory> builder)
    {
        builder.HasKey(lh => lh.Id);
        builder.HasOne(lh => lh.User).WithMany()
               .HasForeignKey(lh => lh.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(lh => lh.Track).WithMany()
               .HasForeignKey(lh => lh.TrackId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(lh => new { lh.UserId, lh.PlayedAt });
        builder.Ignore(lh => lh.DomainEvents);
    }
}
