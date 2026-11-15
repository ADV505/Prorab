using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Prorab.Infrastructure.Data;

/// <summary>Точка входа для dotnet-ef на этапе дизайна.</summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Prorab", "prorab.db");

        return new AppDbContextFactory(dbPath).CreateDbContext();
    }
}
