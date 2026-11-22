using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Prorab.UI.Modules.Common;

namespace Prorab.UI.Modules.WorkOrders;

public partial class WorkOrdersView : UserControl
{
    public WorkOrdersView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private WorkOrdersViewModel? ViewModel => DataContext as WorkOrdersViewModel;

    private Window? OwnerWindow => TopLevel.GetTopLevel(this) as Window;

    private async void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (ViewModel is { } vm) await vm.LoadAsync();
    }

    private async void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is { } vm) await vm.LoadAsync();
    }

    private async void OnNew(object? sender, RoutedEventArgs e) => await OpenEditorAsync(null, null);

    private async void OnOpen(object? sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.CommandParameter is not OrderRow row) return;
        await OpenEditorAsync(row.Id, null);
    }

    private async void OnPayments(object? sender, RoutedEventArgs e)
    {
        if (OwnerWindow is not Window owner)
        {
            if (ViewModel is { } v1) v1.StatusText = "Не найдено родительское окно.";
            return;
        }
        if ((sender as Button)?.CommandParameter is not OrderRow row) return;

        try
        {
            var vm = AppServices.Services.GetRequiredService<WorkOrderPaymentsViewModel>();
            var dialog = new WorkOrderPaymentsWindow { DataContext = vm };
            await vm.LoadAsync(row.Id);
            await dialog.ShowDialog(owner);
            if (ViewModel is { } list) await list.LoadAsync();
        }
        catch (Exception ex)
        {
            if (ViewModel is { } vm)
                vm.StatusText = "Не удалось открыть выплаты: " + ex.Message +
                    (ex.InnerException?.Message is { } inner ? " — " + inner : "");
        }
    }


    private async void OnDelete(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is not { } vm) return;
        if (OwnerWindow is not Window owner) return;
        if ((sender as Button)?.CommandParameter is not OrderRow row) return;

        var confirm = new ConfirmWindow($"Удалить наряд №{row.Number}?");
        if (await confirm.ShowDialog<bool?>(owner) == true)
            await vm.DeleteAsync(row);
    }

    private async Task OpenEditorAsync(int? orderId, int? presetSiteId)
    {
        if (OwnerWindow is not Window owner) return;

        var vm = AppServices.Services.GetRequiredService<WorkOrderEditViewModel>();
        var dialog = new WorkOrderEditWindow { DataContext = vm };
        await vm.LoadAsync(orderId, presetSiteId);
        await dialog.ShowDialog(owner);
        if (ViewModel is { } list) await list.LoadAsync();
    }
}
