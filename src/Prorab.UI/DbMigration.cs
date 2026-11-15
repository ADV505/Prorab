using Microsoft.EntityFrameworkCore;
using Prorab.Infrastructure.Data;

namespace Prorab.UI;

public static class DbMigration
{
    private static bool _done;

    public static async Task EnsureMigratedAsync(IDbContextFactory<AppDbContext> factory)
    {
        if (_done) return;
        _done = true; // однократно за запуск; если упадёт — увидим исключение один раз

        await using var db = await factory.CreateDbContextAsync();

        // --- ОДНОРАЗОВЫЙ КОД для базы, созданной через EnsureCreated ---
        db.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
                "ProductVersion" TEXT NOT NULL
            );
            INSERT OR IGNORE INTO "__EFMigrationsHistory"
                VALUES ('20261115110116_InitialCreate', '10.0.0');
            """);
        // --- конец одноразового кода ---

        await db.Database.MigrateAsync();
    }
}
