using CartMate.Domain.Lists;
using CartMate.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CartMate.Application.Common.Persistence;

/// <summary>
/// What handlers are allowed to see of the database. Implemented in Infrastructure,
/// which keeps Application free of Npgsql and connection details.
/// </summary>
public interface IAppDbContext
{
    DbSet<User> Users { get; }

    DbSet<GroceryList> GroceryLists { get; }

    DbSet<GroceryItem> GroceryItems { get; }

    DbSet<ListMember> ListMembers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
