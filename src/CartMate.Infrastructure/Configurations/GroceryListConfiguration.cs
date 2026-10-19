using CartMate.Domain.Lists;
using CartMate.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartMate.Infrastructure.Persistence.Configurations;

public sealed class GroceryListConfiguration : IEntityTypeConfiguration<GroceryList>
{
    public void Configure(EntityTypeBuilder<GroceryList> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever();

        builder.Property(l => l.Name).HasMaxLength(GroceryList.MaxNameLength).IsRequired();

        builder.Property(l => l.InviteCode)
            .HasMaxLength(InviteCodeGenerator.Length)
            .IsFixedLength()
            .IsRequired();
        builder.HasIndex(l => l.InviteCode).IsUnique();

        // Npgsql maps a uint row version to PostgreSQL's xmin system column:
        // optimistic concurrency without adding a column.
        builder.Property(l => l.Version).IsRowVersion();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(l => l.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.Members)
            .WithOne()
            .HasForeignKey(m => m.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.Items)
            .WithOne()
            .HasForeignKey(i => i.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        // Read and write the private _members/_items fields, not the read-only properties.
        builder.Navigation(l => l.Members).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(l => l.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
