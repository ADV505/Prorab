using Avalonia.Controls;
using Avalonia.Interactivity;
using Prorab.Core.Models;
using Prorab.UI.Modules.Common;

namespace Prorab.UI.Modules.Sites;

public partial class SiteStructureWindow : Window
{
    public SiteStructureWindow() => InitializeComponent();

    private SiteStructureViewModel? ViewModel => DataContext as SiteStructureViewModel;

    private async void OnAddFloor(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is { } vm) await vm.AddFloorAsync(vm.NewFloorName);
    }

    private async void OnEditFloor(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is not { SelectedFloor: Floor floor } vm) return;
        var dialog = new NameEditWindow("Переименовать этаж") { DataContext = new NameEditViewModel { Name = floor.Name } };
        if (await dialog.ShowDialog<bool?>(this) == true)
            await vm.RenameFloorAsync(floor, ((NameEditViewModel)dialog.DataContext!).Name);
    }

    private async void OnDeleteFloor(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is not { SelectedFloor: Floor floor } vm) return;

        if (vm.IsFloorUsed(floor))
        {
            vm.StatusText = "Этаж нельзя удалить: его помещения уже есть в нарядах.";
            return;
        }
        var confirm = new ConfirmWindow($"Удалить этаж «{floor.Name}» вместе с его помещениями?");
        if (await confirm.ShowDialog<bool?>(this) == true)
            await vm.DeleteFloorAsync(floor);
    }

    private async void OnAddRoom(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is { } vm) await vm.AddRoomAsync(vm.NewRoomName, shared: false);
    }

    private async void OnEditRoom(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is not { SelectedRoom: Room room } vm) return;
        var dialog = new NameEditWindow("Переименовать помещение") { DataContext = new NameEditViewModel { Name = room.Name } };
        if (await dialog.ShowDialog<bool?>(this) == true)
            await vm.RenameRoomAsync(room, ((NameEditViewModel)dialog.DataContext!).Name);
    }

    private async void OnDeleteRoom(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is not { SelectedRoom: Room room } vm) return;
        await DeleteWithConfirmAsync(room);
    }

    private async void OnAddSharedRoom(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is { } vm) await vm.AddRoomAsync(vm.NewSharedRoomName, shared: true);
    }

    private async void OnEditSharedRoom(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is not { SelectedSharedRoom: Room room } vm) return;
        var dialog = new NameEditWindow("Переименовать общее помещение") { DataContext = new NameEditViewModel { Name = room.Name } };
        if (await dialog.ShowDialog<bool?>(this) == true)
            await vm.RenameRoomAsync(room, ((NameEditViewModel)dialog.DataContext!).Name);
    }

    private async void OnDeleteSharedRoom(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is not { SelectedSharedRoom: Room room } vm) return;
        await DeleteWithConfirmAsync(room);
    }

    private async void OnClose(object? sender, RoutedEventArgs e) => Close(true);

    private async Task DeleteWithConfirmAsync(Room room)
    {
        if (ViewModel is not { } vm) return;

        if (vm.IsRoomUsed(room))
        {
            vm.StatusText = $"Помещение «{room.Name}» уже есть в нарядах — удалить нельзя.";
            return;
        }
        var confirm = new ConfirmWindow($"Удалить помещение «{room.Name}»?");
        if (await confirm.ShowDialog<bool?>(this) == true)
            await vm.DeleteRoomAsync(room);
    }

}
