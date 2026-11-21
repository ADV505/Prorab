using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;

namespace Prorab.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Crew> Crews => Set<Crew>();
    public DbSet<CrewWorker> CrewWorkers => Set<CrewWorker>();
    public DbSet<CrewSite> CrewSites => Set<CrewSite>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<WorkType> WorkTypes => Set<WorkType>();
    public DbSet<Worker> Workers => Set<Worker>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Crew>().HasIndex(c => c.Name);
        modelBuilder.Entity<Site>().HasIndex(s => s.Name);
        modelBuilder.Entity<WorkType>().HasIndex(w => w.Name);

        modelBuilder.Entity<Crew>()
            .HasOne(c => c.Foreman)
            .WithMany()
            .HasForeignKey(c => c.ForemanWorkerId);

        modelBuilder.Entity<CrewWorker>(b =>
        {
            b.HasKey(cw => new { cw.CrewId, cw.WorkerId });
            b.HasOne(cw => cw.Crew).WithMany(c => c.Members).HasForeignKey(cw => cw.CrewId);
            b.HasOne(cw => cw.Worker).WithMany().HasForeignKey(cw => cw.WorkerId);
        });

        modelBuilder.Entity<CrewSite>(b =>
        {
            b.HasKey(cs => new { cs.CrewId, cs.SiteId });
            b.HasOne(cs => cs.Crew).WithMany(c => c.Sites).HasForeignKey(cs => cs.CrewId);
            b.HasOne(cs => cs.Site).WithMany().HasForeignKey(cs => cs.SiteId);
        });
    }



}
