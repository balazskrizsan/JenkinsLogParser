using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JLP.Factories;

public class ContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var option = new DbContextOptionsBuilder<AppDbContext>();
        option.UseNpgsql("Host=localhost;Database=stackjudge;Port=54330;Username=admin;Password=admin_pass;");

        return new AppDbContext(option.Options);
    }
}