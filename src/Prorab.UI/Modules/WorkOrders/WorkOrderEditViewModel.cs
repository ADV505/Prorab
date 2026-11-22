using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;
using Prorab.Infrastructure.Data;
using Prorab.UI.ViewModels;
using System.Collections.ObjectModel;

namespace Prorab.UI.Modules.WorkOrders;

/// <summary>Обёртка помещения для чек-листа выбора.</summary>
public partial class RoomPickItem : ViewModelBase
{
    public Room Room { get; }
    [ObservableProperty] private bool _isSelected;
    public string Display { get; }

    public RoomPickItem(Room room, string display)
    {
        Room = room;
        Display = display;
    }
}

/// <summary>Одна строка позиции наряда.</summary>
public partial class WorkOrderItemRow : ViewModelBase
{
    public ObservableCollection<Room> Rooms { get; }
    public ObservableCollection<WorkType> WorkTypes { get; }
    public ObservableCollection<string> SectionSuggestions { get; }

    [ObservableProperty] private Room? _selectedRoom;
    [ObservableProperty] private string _sectionName = "";
    [ObservableProperty] private WorkType? _selectedWorkType;
    [ObservableProperty] private string _quantityText = "1";
    [ObservableProperty] private string _unitPriceText = "";
    [ObservableProperty] private string _surchargeText = "";
    [ObservableProperty] private string _discountText = "";

    public WorkOrderItemRow(
        ObservableCollection<Room> rooms,
        ObservableCollection<WorkType> workTypes,
        ObservableCollection<string> sectionSuggestions)
    {
        Rooms = rooms;
        WorkTypes = workTypes;
        SectionSuggestions = sectionSuggestions;
    }

    public bool TryGetTotal(out decimal total)
    {
        total = 0;
        if (SelectedWorkType is null) return false;
        if (!decimal.TryParse(QuantityText, out var q)) return false;
        if (!decimal.TryParse(UnitPriceText, out var p)) return false;
        decimal.TryParse(SurchargeText, out var s);
        decimal.TryParse(DiscountText, out var d);
        total = Math.Round(q * p * (1 + s / 100m) * (1 - d / 100m), 2);
        return true;
    }

    public string LineTotalText => TryGetTotal(out var t) ? t.ToString("0.##") : "—";

    public void RefreshTotal() => OnPropertyChanged(nameof(LineTotalText));

    partial void OnSelectedWorkTypeChanged(WorkType? value) => RefreshTotal();
    partial void OnQuantityTextChanged(string value) => RefreshTotal();
    partial void OnUnitPriceTextChanged(string value) => RefreshTotal();
    partial void OnSurchargeTextChanged(string value) => RefreshTotal();
    partial void OnDiscountTextChanged(string value) => RefreshTotal();
}

public partial class WorkOrderEditViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private int? _orderId; // null — новый наряд

    public ObservableCollection<Crew> Crews { get; } = [];
    public ObservableCollection<Site> Sites { get; } = [];
    public ObservableCollection<Room> AllRooms { get; } = [];
    public ObservableCollection<RoomPickItem> RoomPicks { get; } = [];
    public ObservableCollection<WorkType> WorkTypes { get; } = [];
    public ObservableCollection<string> SectionSuggestions { get; } = [];
    public ObservableCollection<WorkOrderItemRow> Rows { get; } = [];

    [ObservableProperty] private Crew? _selectedCrew;
    [ObservableProperty] private Site? _selectedSite;
    [ObservableProperty] private DateTimeOffset? _date = DateTimeOffset.Now.Date;
    [ObservableProperty] private string _number = "";
    [ObservableProperty] private string _statusText = "";

    public string TotalText =>
        Rows.Aggregate(0m, (sum, r) => r.TryGetTotal(out var t) ? sum + t : sum)
            .ToString("0.##");

    public WorkOrderEditViewModel(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    partial void OnSelectedSiteChanged(Site? value) => _ = LoadRoomsAsync();

    public async Task LoadAsync(int? orderId = null, int? presetSiteId = null)
    {
        _orderId = orderId;

        await using var db = await _factory.CreateDbContextAsync();
        await DbMigration.EnsureMigratedAsync(_factory);

        var crews = await db.Crews
            .Where(c => !c.IsArchived)
            .OrderBy(c => c.Name)
            .ToListAsync();
        Crews.Clear();
        foreach (var c in crews) Crews.Add(c);

        var sites = await db.Sites
            .Where(s => !s.IsArchived)
            .OrderBy(s => s.Name)
            .ToListAsync();
        Sites.Clear();
        foreach (var s in sites) Sites.Add(s);

        var types = await db.WorkTypes
            .OrderBy(w => w.Name)
            .ToListAsync();
        WorkTypes.Clear();
        foreach (var w in types) WorkTypes.Add(w);

        var sections = await db.WorkOrderItems
            .Select(i => i.SectionName)
            .Distinct()
            .ToListAsync();
        SectionSuggestions.Clear();
        foreach (var s in sections.Where(s => !string.IsNullOrWhiteSpace(s)).OrderBy(s => s))
            SectionSuggestions.Add(s);

        if (_orderId is null)
        {
            var count = await db.WorkOrders.CountAsync();
            Number = $"{count + 1}/{DateTime.Today:dd.MM}";
            if (presetSiteId is int sid)
                SelectedSite = Sites.FirstOrDefault(s => s.Id == sid);
        }
        else
        {
            var order = await db.WorkOrders
                .Include(o => o.Items)
                .FirstAsync(o => o.Id == _orderId);

            Number = order.Number;
            Date = new DateTimeOffset(order.Date); // если в модели DateOnly — покажи WorkOrder.cs, поправлю
            SelectedCrew = Crews.FirstOrDefault(c => c.Id == order.CrewId);
            SelectedSite = Sites.FirstOrDefault(s => s.Id == order.SiteId);

            await LoadRoomsAsync();

            foreach (var item in order.Items)
            {
                var row = NewRow();
                row.SelectedRoom = AllRooms.FirstOrDefault(r => r.Id == item.RoomId);
                row.SectionName = item.SectionName;
                row.SelectedWorkType = WorkTypes.FirstOrDefault(w => w.Id == item.WorkTypeId);
                row.QuantityText = item.Quantity.ToString("0.##");
                row.UnitPriceText = item.UnitPrice.ToString("0.##");
                if (item.SurchargePercent is { } sur) row.SurchargeText = sur.ToString("0.##");
                if (item.DiscountPercent is { } disc) row.DiscountText = disc.ToString("0.##");
                Rows.Add(row);

                var pick = RoomPicks.FirstOrDefault(p => p.Room.Id == item.RoomId);
                if (pick is not null) pick.IsSelected = true;
            }
        }
    }

    private async Task LoadRoomsAsync()
    {
        AllRooms.Clear();
        RoomPicks.Clear();
        if (SelectedSite is null) return;

        await using var db = await _factory.CreateDbContextAsync();
        var rooms = await db.Rooms
            .Where(r => r.SiteId == SelectedSite.Id && !r.IsArchived)
            .Include(r => r.Floor)
            .ToListAsync();

        rooms = rooms
            .OrderBy(r => NameNum(r.Floor?.Name))
            .ThenBy(r => NameNum(r.Name))
            .ToList();

        foreach (var room in rooms)
        {
            AllRooms.Add(room);
            RoomPicks.Add(new RoomPickItem(room,
                room.Floor is null ? $"Общие · {room.Name}" : $"{room.Floor.Name} · {room.Name}"));
        }
    }

    // ---------- Позиции ----------

    public void AddRows()
    {
        StatusText = "";
        var selected = RoomPicks.Where(p => p.IsSelected).Select(p => p.Room).ToList();

        if (selected.Count == 0)
        {
            // без отмеченных помещений — одна пустая строка, помещение выберется в строке
            Rows.Add(NewRow());
            return;
        }

        foreach (var room in selected)
        {
            var row = NewRow();
            row.SelectedRoom = room;
            Rows.Add(row);
        }
    }

    public void RemoveRow(WorkOrderItemRow row) => Rows.Remove(row);

    private WorkOrderItemRow NewRow()
    {
        var row = new WorkOrderItemRow(AllRooms, WorkTypes, SectionSuggestions);
        row.PropertyChanged += (_, _) => OnPropertyChanged(nameof(TotalText));
        return row;
    }

    // ---------- Сохранение ----------

    public async Task<bool> SaveAsync()
    {
        StatusText = "";

        if (SelectedCrew is null) { StatusText = "Выберите бригаду."; return false; }
        if (SelectedSite is null) { StatusText = "Выберите объект."; return false; }
        if (Date is null) { StatusText = "Укажите дату."; return false; }
        if (Rows.Count == 0) { StatusText = "Добавьте хотя бы одну позицию."; return false; }

        for (var i = 0; i < Rows.Count; i++)
        {
            var row = Rows[i];
            if (row.SelectedRoom is null)
            { StatusText = $"Строка {i + 1}: не выбрано помещение."; return false; }
            if (row.SelectedWorkType is null)
            { StatusText = $"Строка {i + 1}: не выбран вид работ."; return false; }
            if (!row.TryGetTotal(out _))
            { StatusText = $"Строка {i + 1}: проверьте количество и цену."; return false; }
        }

        await using var db = await _factory.CreateDbContextAsync();

        WorkOrder order;
        if (_orderId is null)
        {
            var count = await db.WorkOrders.CountAsync();
            order = new WorkOrder
            {
                Number = $"{count + 1}/{Date.Value.DateTime:dd.MM}",
            };
            db.WorkOrders.Add(order);
        }
        else
        {
            order = await db.WorkOrders
                .Include(o => o.Items)
                .FirstAsync(o => o.Id == _orderId);
            db.WorkOrderItems.RemoveRange(order.Items);
            order.Items.Clear();
        }

        order.CrewId = SelectedCrew.Id;
        order.SiteId = SelectedSite.Id;
        order.Date = Date.Value.DateTime;

        foreach (var row in Rows)
        {
            order.Items.Add(new WorkOrderItem
            {
                RoomId = row.SelectedRoom!.Id,
                SectionName = row.SectionName.Trim(),
                WorkTypeId = row.SelectedWorkType!.Id,
                Quantity = decimal.Parse(row.QuantityText),
                UnitPrice = decimal.Parse(row.UnitPriceText),
                SurchargePercent = decimal.TryParse(row.SurchargeText, out var s) ? s : null,
                DiscountPercent = decimal.TryParse(row.DiscountText, out var d) ? d : null,
            });
        }

        await db.SaveChangesAsync();
        return true;
    }

    // ---------- Служебное ----------

    private static int NameNum(string? name) =>
        int.TryParse(name, out var n) ? n : int.MaxValue;
}
