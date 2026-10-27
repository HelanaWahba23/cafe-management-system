using CafeManagement.Api.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Api.Infrastructure.Persistence;

public sealed class CafeDbContext(DbContextOptions<CafeDbContext> options) : IdentityDbContext(options)
{
    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CafeTable> CafeTables => Set<CafeTable>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(CafeDbContext).Assembly);
    }
}
