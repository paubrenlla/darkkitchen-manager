using DarkKitchen.Domain;
using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess;

public class DarkKitchenContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductLine> ProductLines { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Promotion> Promotions { get; set; }

    public DarkKitchenContext(DbContextOptions<DarkKitchenContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DarkKitchenContext).Assembly);
    }
}
