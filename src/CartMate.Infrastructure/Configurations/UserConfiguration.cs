using CartMate.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartMate.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        // IDs are generated in the domain (UUIDv7), never by EF or the database.
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.Email).HasMaxLength(User.MaxEmailLength).IsRequired();
        builder.Property(u => u.NormalizedEmail).HasMaxLength(User.MaxEmailLength).IsRequired();
        builder.HasIndex(u => u.NormalizedEmail).IsUnique();

        builder.Property(u => u.DisplayName).HasMaxLength(User.MaxDisplayNameLength).IsRequired();
        builder.Property(u => u.PasswordHash).IsRequired();
    }
}
