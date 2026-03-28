using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicApp.Domain.Entities;

namespace MusicApp.Infrastructure.Persistence.Configurations;

public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Bio).HasMaxLength(2000);
        builder.Property(a => a.ImageUrl).HasMaxLength(500);
        builder.HasOne(a => a.User).WithOne()
               .HasForeignKey<Artist>(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Ignore(a => a.DomainEvents);
    }
}
