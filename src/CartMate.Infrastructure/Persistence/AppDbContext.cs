using CartMate.Application.Common.Persistence;
using CartMate.Domain.Lists;
using CartMate.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CartMate.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<User> Users => Set<User>();

    public DbSet<GroceryList> GroceryLists => Set<GroceryList>();

    public DbSet<GroceryItem> GroceryItems => Set<GroceryItem>();

    public DbSet<ListMember> ListMembers => Set<ListMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
