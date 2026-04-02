using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicApp.Domain.Entities;

namespace MusicApp.Infrastructure.Persistence.Configurations;

public class SearchHistoryConfiguration : IEntityTypeConfiguration<SearchHistory>
{
    public void Configure(EntityTypeBuilder<SearchHistory> builder)
    {
        builder.HasKey(sh => sh.Id);
        builder.HasOne(sh => sh.User).WithMany()
               .HasForeignKey(sh => sh.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(sh => sh.Track).WithMany()
               .HasForeignKey(sh => sh.TrackId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(sh => new { sh.UserId, sh.SearchedAt });
        builder.HasIndex(sh => new { sh.UserId, sh.TrackId }).IsUnique();
        builder.Ignore(sh => sh.DomainEvents);
    }
}
