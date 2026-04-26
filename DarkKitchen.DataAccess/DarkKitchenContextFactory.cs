using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DarkKitchen.DataAccess;

public class DarkKitchenContextFactory : IDesignTimeDbContextFactory<DarkKitchenContext>
{
    public DarkKitchenContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DarkKitchenContext>();
        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=DarkKitchenDB;User Id=sa;Password=DarkKitchen2026!;TrustServerCertificate=true");
        return new DarkKitchenContext(optionsBuilder.Options);
    }
}
