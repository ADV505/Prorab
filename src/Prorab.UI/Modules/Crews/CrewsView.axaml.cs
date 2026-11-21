using Avalonia.Controls;
using Avalonia.Interactivity;
using Prorab.Core.Models;
using Prorab.UI.Modules.Crews;

namespace Prorab.UI.Modules.Crews;

public partial class CrewsView : UserControl
{
    public CrewsView() => InitializeComponent();

    private async void OnAdd(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not CrewsViewModel vm) return;
        if (TopLevel.GetTopLevel(this) is not Window owner) return;
        await vm.AddAsync(owner);
    }

    private async void OnEdit(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not CrewsViewModel vm) return;
        if (TopLevel.GetTopLevel(this) is not Window owner) return;
        if (e.Source is Button { CommandParameter: Crew crew })
            await vm.EditAsync(crew, owner);
    }

    private async void OnArchive(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not CrewsViewModel vm) return;
        if (e.Source is Button { CommandParameter: Crew crew })
            await vm.ArchiveAsync(crew);
    }

    private async void OnRestore(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not CrewsViewModel vm) return;
        if (e.Source is Button { CommandParameter: Crew crew })
            await vm.RestoreAsync(crew);
    }
}
