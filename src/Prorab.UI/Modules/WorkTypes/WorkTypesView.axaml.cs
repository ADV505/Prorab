using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Prorab.Core.Models;
using Prorab.UI.Modules.Common;

namespace Prorab.UI.Modules.WorkTypes;

public partial class WorkTypesView : UserControl
{
    public WorkTypesView()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private Window? OwnerWindow => TopLevel.GetTopLevel(this) as Window;

    private async void OnAdd(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not WorkTypesViewModel viewModel) return;
        if (OwnerWindow is not Window owner) return;

        var edit = new WorkTypeEditViewModel();
        var dialog = new WorkTypeEditWindow("Новый вид работ") { DataContext = edit };

        if (await dialog.ShowDialog<bool?>(owner) == true)
        {
            decimal.TryParse(edit.BasePrice?.Replace('.', ','), out var basePrice);
            await viewModel.AddAsync(edit.Name, edit.Unit, basePrice);
        }
    }

    private async void OnEdit(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not WorkTypesViewModel viewModel) return;
        if ((sender as Button)?.CommandParameter is not WorkType workType) return;
        if (OwnerWindow is not Window owner) return;

        var edit = new WorkTypeEditViewModel
        {
            Name = workType.Name,
            Unit = workType.Unit,
            BasePrice = workType.BasePrice == 0 ? "" : workType.BasePrice.ToString("0.##"),
        };
        var dialog = new WorkTypeEditWindow("Изменить вид работ") { DataContext = edit };

        if (await dialog.ShowDialog<bool?>(owner) == true)
        {
            decimal.TryParse(edit.BasePrice?.Replace('.', ','), out var basePrice);
            await viewModel.UpdateAsync(workType, edit.Name, edit.Unit, basePrice);
        }
    }

    private async void OnArchive(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not WorkTypesViewModel viewModel) return;
        if ((sender as Button)?.CommandParameter is not WorkType workType) return;
        if (OwnerWindow is not Window owner) return;

        var confirm = new ConfirmWindow($"Архивировать вид работ «{workType.Name}»? Он исчезнет из списка, но данные сохранятся.");
        if (await confirm.ShowDialog<bool?>(owner) == true)
            await viewModel.ArchiveAsync(workType);
    }
}
