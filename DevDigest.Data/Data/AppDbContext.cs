using DevDigest.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DevDigest.Data.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Article> Articles => Set<Article>();
}