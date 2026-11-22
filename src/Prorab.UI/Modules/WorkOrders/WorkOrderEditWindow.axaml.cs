using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Prorab.UI.Modules.WorkOrders;

public partial class WorkOrderEditWindow : Window
{
    public WorkOrderEditWindow() => InitializeComponent();

    private WorkOrderEditViewModel? ViewModel => DataContext as WorkOrderEditViewModel;

    private void OnAddRows(object? sender, RoutedEventArgs e) => ViewModel?.AddRows();

    private void OnRemoveRow(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is { } vm && (sender as Button)?.CommandParameter is WorkOrderItemRow row)
            vm.RemoveRow(row);
    }

    private async void OnSave(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is not { } vm) return;
        if (await vm.SaveAsync()) Close(true);
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}
