using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;
using Prorab.Infrastructure.Data;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Sites;

public partial class SiteStructureViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    private int _siteId;
    private List<Room> _allRooms = [];
    private HashSet<int> _usedRoomIds = [];

    public ObservableCollection<Floor> Floors { get; } = [];
    public ObservableCollection<Room> Rooms { get; } = [];
    public ObservableCollection<Room> SharedRooms { get; } = [];

    [ObservableProperty] private Floor? _selectedFloor;
    [ObservableProperty] private Room? _selectedRoom;
    [ObservableProperty] private Room? _selectedSharedRoom;

    [ObservableProperty] private string _newFloorName = "";
    [ObservableProperty] private string _newRoomName = "";
    [ObservableProperty] private string _newSharedRoomName = "";
    [ObservableProperty] private string _statusText = "";

    public string SiteTitle { get; private set; } = "";
    public string RoomsHeader => SelectedFloor is null ? "Помещения" : $"Помещения — {SelectedFloor.Name}";

    public SiteStructureViewModel(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    partial void OnSelectedFloorChanged(Floor? value) => RefreshRooms();

    public async Task LoadAsync(int siteId, string siteName)
    {
        _siteId = siteId;
        SiteTitle = siteName;
        OnPropertyChanged(nameof(SiteTitle));

        await using var db = await _factory.CreateDbContextAsync();
        await DbMigration.EnsureMigratedAsync(_factory);

        var floors = await db.Floors
            .Where(f => f.SiteId == siteId && !f.IsArchived)
            .ToListAsync();

        Floors.Clear();
        foreach (var f in floors) Floors.Add(f);
        SortFloors(Floors);

        _allRooms = await db.Rooms
            .Where(r => r.SiteId == siteId && !r.IsArchived)
            .ToListAsync();

        SharedRooms.Clear();
        foreach (var r in _allRooms.Where(r => r.FloorId is null).OrderBy(NumericOrder))
            SharedRooms.Add(r);

        _usedRoomIds = (await db.WorkOrderItems
            .Select(i => i.RoomId)
            .Distinct()
            .ToListAsync()).ToHashSet();

        SelectedFloor = null;
        RefreshRooms();
    }

    // ---------- Этажи ----------

    public async Task AddFloorAsync(string name)
    {
        name = name.Trim();
        if (name.Length == 0) return;
        StatusText = "";

        var floor = new Floor { SiteId = _siteId, Name = name };

        await using var db = await _factory.CreateDbContextAsync();
        db.Floors.Add(floor);
        await db.SaveChangesAsync();

        Floors.Add(floor);
        SortFloors(Floors);
        NewFloorName = "";
    }

    public async Task RenameFloorAsync(Floor floor, string name)
    {
        name = name.Trim();
        if (name.Length == 0) return;

        floor.Name = name;

        await using var db = await _factory.CreateDbContextAsync();
        db.Floors.Update(floor);
        await db.SaveChangesAsync();

        SortFloors(Floors);
        OnPropertyChanged(nameof(RoomsHeader));
    }

    public bool IsFloorUsed(Floor floor) =>
        _allRooms.Any(r => r.FloorId == floor.Id && _usedRoomIds.Contains(r.Id));

    public async Task DeleteFloorAsync(Floor floor)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Floors.Remove(floor);
        await db.SaveChangesAsync();

        var removed = _allRooms.Where(r => r.FloorId == floor.Id).ToList();
        foreach (var room in removed) _allRooms.Remove(room);
        foreach (var room in removed) SharedRooms.Remove(room);

        Floors.Remove(floor);
        if (SelectedFloor == floor)
        {
            SelectedFloor = null;
            RefreshRooms();
        }
    }

    // ---------- Помещения ----------

    public async Task AddRoomAsync(string name, bool shared)
    {
        name = name.Trim();
        if (name.Length == 0) return;

        if (!shared && SelectedFloor is null)
        {
            StatusText = "Выберите этаж слева — или добавляйте помещение в «Общие».";
            return;
        }
        StatusText = "";

        var room = new Room
        {
            SiteId = _siteId,
            FloorId = shared ? null : SelectedFloor!.Id,
            Name = name,
        };

        await using var db = await _factory.CreateDbContextAsync();
        db.Rooms.Add(room);
        await db.SaveChangesAsync();

        _allRooms.Add(room);
        if (shared)
        {
            SharedRooms.Add(room);
            NewSharedRoomName = "";
        }
        else
        {
            RefreshRooms();
            NewRoomName = "";
        }
    }

    public async Task RenameRoomAsync(Room room, string name)
    {
        name = name.Trim();
        if (name.Length == 0) return;

        room.Name = name;

        await using var db = await _factory.CreateDbContextAsync();
        db.Rooms.Update(room);
        await db.SaveChangesAsync();

        RefreshRooms();
        if (SharedRooms.Contains(room))
        {
            SharedRooms[SharedRooms.IndexOf(room)] = room; // обновляем текст строки
            SortSharedRooms();
        }
    }

    public bool IsRoomUsed(Room room) => _usedRoomIds.Contains(room.Id);

    public async Task DeleteRoomAsync(Room room)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Rooms.Remove(room);
        await db.SaveChangesAsync();

        _allRooms.Remove(room);
        SharedRooms.Remove(room);
        if (SelectedRoom == room) SelectedRoom = null;
        if (SelectedSharedRoom == room) SelectedSharedRoom = null;
        RefreshRooms();
    }

    // ---------- Служебное ----------

    private void RefreshRooms()
    {
        Rooms.Clear();
        if (SelectedFloor is null) return;

        foreach (var room in _allRooms
                     .Where(r => r.FloorId == SelectedFloor.Id)
                     .OrderBy(NumericOrder))
        {
            Rooms.Add(room);
        }
        OnPropertyChanged(nameof(RoomsHeader));
    }

    private void SortSharedRooms()
    {
        var ordered = SharedRooms.OrderBy(NumericOrder).ToList();
        for (var i = 0; i < ordered.Count; i++)
        {
            if (SharedRooms[i] != ordered[i])
                SharedRooms.Move(SharedRooms.IndexOf(ordered[i]), i);
        }
    }

    private void SortFloors(ObservableCollection<Floor> floors)
    {
        var ordered = floors
            .OrderBy(FloorNum)
            .ThenBy(f => f.Name, StringComparer.Ordinal)
            .ToList();

        for (var i = 0; i < ordered.Count; i++)
        {
            if (floors[i] != ordered[i])
                floors.Move(floors.IndexOf(ordered[i]), i);
        }
    }

    private static int FloorNum(Floor floor) =>
        int.TryParse(floor.Name, out var n) ? n : int.MaxValue;

    private static int NumericOrder(Room room) =>
        int.TryParse(room.Name, out var n) ? n : int.MaxValue;
}
