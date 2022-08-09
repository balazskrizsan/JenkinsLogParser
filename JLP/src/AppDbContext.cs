using JLP.Entities;
using Microsoft.EntityFrameworkCore;

namespace JLP;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContext) : base(dbContext)
    {
    }

    public DbSet<Log> Logs { get; set; }
    public DbSet<Error> Errors { get; set; }
}