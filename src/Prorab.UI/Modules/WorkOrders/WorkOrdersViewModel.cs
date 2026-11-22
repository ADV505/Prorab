using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;
using Prorab.Infrastructure.Data;
using Prorab.UI.ViewModels;
using System.Collections.ObjectModel;

namespace Prorab.UI.Modules.WorkOrders;

/// <summary>Строка списка нарядов.</summary>
public partial class OrderRow : ViewModelBase
{
    public int Id { get; init; }
    public string Number { get; init; } = "";
    public DateTime Date { get; init; }
    public string CrewName { get; init; } = "";
    public string SiteName { get; init; } = "";
    public decimal Total { get; init; }
    public decimal Paid { get; init; }
    public decimal Remaining { get; init; }
    public decimal Official { get; init; }

    public string DateText => Date.ToString("dd.MM.yyyy");
    public string TotalText => Total.ToString("0.##");
    public string PaidText => Paid.ToString("0.##");
    public string RemainingText => Remaining.ToString("0.##");
    public string OfficialText => Official.ToString("0.##");
}

public partial class WorkOrdersViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public ObservableCollection<OrderRow> Orders { get; } = [];
    [ObservableProperty] private string _statusText = "";

    public WorkOrdersViewModel(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    public async Task LoadAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        await DbMigration.EnsureMigratedAsync(_factory);

        var orders = await db.WorkOrders
            .Include(o => o.Crew)
            .Include(o => o.Site)
            .Include(o => o.Items)
            .Include(o => o.Payments)
            .Where(o => !o.IsArchived)
            .OrderByDescending(o => o.Date)
            .ToListAsync();

        Orders.Clear();
        foreach (var o in orders)
        {
            Orders.Add(new OrderRow
            {
                Id = o.Id,
                Number = o.Number,
                Date = o.Date,
                CrewName = o.Crew?.Name ?? "",
                SiteName = o.Site?.Name ?? "",
                Total = o.GrandTotal,
                Paid = o.PaidTotal,
                Remaining = o.Remaining,
                Official = o.Payments
    .Where(p => p.Method == PaymentMethod.BankTransfer)
    .Sum(p => p.Amount),
            });
        }
    }

    public async Task<bool> DeleteAsync(OrderRow row)
    {
        if (row.Paid > 0)
        {
            StatusText = "По наряду есть выплаты — сначала удалите их в окне выплат.";
            return false;
        }

        await using var db = await _factory.CreateDbContextAsync();
        var order = await db.WorkOrders
            .Include(o => o.Items)
            .FirstAsync(o => o.Id == row.Id);
        db.WorkOrderItems.RemoveRange(order.Items);
        db.WorkOrders.Remove(order);
        await db.SaveChangesAsync();

        Orders.Remove(row);
        return true;
    }
}
