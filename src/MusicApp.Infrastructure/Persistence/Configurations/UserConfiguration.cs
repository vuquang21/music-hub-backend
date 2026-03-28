using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicApp.Domain.Entities;

namespace MusicApp.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
        builder.Property(u => u.DisplayName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.AvatarUrl).HasMaxLength(500);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasMany(u => u.RefreshTokens).WithOne()
               .HasForeignKey(rt => rt.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.LikedTracks).WithMany()
               .UsingEntity(j => j.ToTable("UserLikedTracks"));
        builder.HasMany(u => u.Following).WithMany(u => u.Followers)
               .UsingEntity(j => j.ToTable("UserFollows"));
        builder.HasQueryFilter(u => u.IsActive);
        builder.Ignore(u => u.DomainEvents);
    }
}
