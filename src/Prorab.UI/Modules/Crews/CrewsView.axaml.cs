using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Prorab.Core.Models;

namespace Prorab.UI.Modules.Crews;

public partial class CrewsView : UserControl
{
    public CrewsView()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private Window? OwnerWindow => TopLevel.GetTopLevel(this) as Window;

    private async void OnAdd(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not CrewsViewModel viewModel) return;
        if (OwnerWindow is not Window owner) return;

        var edit = new CrewEditViewModel();
        var dialog = new CrewEditWindow { DataContext = edit };

        if (await dialog.ShowDialog<bool?>(owner) == true)
            await viewModel.AddAsync(edit.Name, edit.ForemanName, edit.Phone);
    }
    private async void OnEdit(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not CrewsViewModel viewModel) return;
        if ((sender as Button)?.CommandParameter is not Crew crew) return;
        if (OwnerWindow is not Window owner) return;

        var edit = new CrewEditViewModel
        {
            Name = crew.Name,
            ForemanName = crew.ForemanName,
            Phone = crew.Phone,
        };
        var dialog = new CrewEditWindow("Изменить бригаду") { DataContext = edit };

        if (await dialog.ShowDialog<bool?>(owner) == true)
            await viewModel.UpdateAsync(crew, edit.Name, edit.ForemanName, edit.Phone);
    }

    private async void OnArchive(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not CrewsViewModel viewModel) return;
        if ((sender as Button)?.CommandParameter is not Crew crew) return;
        if (OwnerWindow is not Window owner) return;

        var confirm = new ConfirmWindow($"Архивировать бригаду «{crew.Name}»? Она исчезнет из списка, но данные сохранятся.");
        if (await confirm.ShowDialog<bool?>(owner) == true)
            await viewModel.ArchiveAsync(crew);
    }


}
