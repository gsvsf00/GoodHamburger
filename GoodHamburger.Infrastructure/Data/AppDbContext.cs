using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
}