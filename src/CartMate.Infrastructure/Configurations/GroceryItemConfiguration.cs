using CartMate.Domain.Lists;
using CartMate.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartMate.Infrastructure.Persistence.Configurations;

public sealed class GroceryItemConfiguration : IEntityTypeConfiguration<GroceryItem>
{
    public void Configure(EntityTypeBuilder<GroceryItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();

        builder.Property(i => i.Name).HasMaxLength(GroceryItem.MaxNameLength).IsRequired();
        builder.Property(i => i.Category).HasMaxLength(GroceryItem.MaxCategoryLength);
        builder.Property(i => i.Note).HasMaxLength(GroceryItem.MaxNoteLength);

        builder.Property(i => i.Version).IsRowVersion();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(i => i.AddedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(i => i.BoughtByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // The list screen loads items per list, unbought first.
        builder.HasIndex(i => new { i.ListId, i.IsBought });
    }
}
