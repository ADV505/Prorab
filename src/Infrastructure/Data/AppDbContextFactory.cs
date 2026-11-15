using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Prorab.Infrastructure.Data;

public class AppDbContextFactory(string dbPath) : IDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .Options;
        return new AppDbContext(options);
    }
}
