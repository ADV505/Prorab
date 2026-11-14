using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;

namespace Prorab.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Crew> Crews => Set<Crew>();

    protected override void OnModelCreating(ModelBuilder b)
        => b.Entity<Crew>().HasIndex(c => c.Name);
}
