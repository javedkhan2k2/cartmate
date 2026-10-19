using CartMate.Domain.Lists;
using CartMate.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartMate.Infrastructure.Persistence.Configurations;

public sealed class ListMemberConfiguration : IEntityTypeConfiguration<ListMember>
{
    public void Configure(EntityTypeBuilder<ListMember> builder)
    {
        // A user is a member of a given list at most once.
        builder.HasKey(m => new { m.ListId, m.UserId });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // "Which lists am I in?" is a common query.
        builder.HasIndex(m => m.UserId);
    }
}
