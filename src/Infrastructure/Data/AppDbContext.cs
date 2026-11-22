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
    public DbSet<Floor> Floors => Set<Floor>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderItem> WorkOrderItems => Set<WorkOrderItem>();
    public DbSet<WorkOrderPayment> WorkOrderPayments => Set<WorkOrderPayment>();


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

        

        modelBuilder.Entity<Floor>(b =>
        {
            b.HasOne(f => f.Site).WithMany().HasForeignKey(f => f.SiteId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(f => f.SiteId);
        });

        modelBuilder.Entity<Room>(b =>
        {
            b.HasOne(r => r.Site).WithMany().HasForeignKey(r => r.SiteId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(r => r.SiteId);
            b.HasOne(r => r.Floor).WithMany(f => f.Rooms).HasForeignKey(r => r.FloorId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(r => r.FloorId);
        });



        modelBuilder.Entity<WorkOrder>(b =>
        {
            b.HasIndex(o => o.Number);
            b.HasIndex(o => o.Date);
            b.HasOne(o => o.Crew).WithMany().HasForeignKey(o => o.CrewId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(o => o.Site).WithMany().HasForeignKey(o => o.SiteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WorkOrderItem>(b =>
        {
            b.HasOne(i => i.WorkOrder).WithMany(o => o.Items).HasForeignKey(i => i.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(i => i.Room).WithMany().HasForeignKey(i => i.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(i => i.WorkType).WithMany().HasForeignKey(i => i.WorkTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            b.Property(i => i.SectionName).HasMaxLength(100);
            b.HasIndex(i => i.SectionName);
        });

        modelBuilder.Entity<WorkOrderPayment>(b =>
        {
            b.HasOne(p => p.WorkOrder).WithMany(o => o.Payments).HasForeignKey(p => p.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(p => p.Worker).WithMany().HasForeignKey(p => p.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

    }



}
