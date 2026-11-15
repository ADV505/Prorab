using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

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
}
