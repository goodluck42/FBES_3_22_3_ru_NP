using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace StoreApi;

class StoreDbContext : DbContext
{
    private IConfiguration _configuration;
    
    public StoreDbContext(DbContextOptions<StoreDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
        Database.EnsureCreated();
    }

    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:StoreDb"]);
    }
}