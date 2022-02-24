using lebonanimal.Models;
using Microsoft.EntityFrameworkCore;

namespace lebonanimal.Datas;

public class DbConnect: DbContext
{
    public DbConnect(DbContextOptions<DbConnect> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories{ get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
    }
}

