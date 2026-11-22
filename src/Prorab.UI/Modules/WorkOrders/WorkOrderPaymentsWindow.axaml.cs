using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Prorab.UI.Modules.WorkOrders;

public partial class WorkOrderPaymentsWindow : Window
{
    public WorkOrderPaymentsWindow() => InitializeComponent();

    private async void OnAddPayment(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (DataContext is not WorkOrderPaymentsViewModel vm)
            {
                Title = "Ошибка: DataContext не является WorkOrderPaymentsViewModel";
                return;
            }
            await vm.AddPaymentAsync();
        }
        catch (Exception ex)
        {
            Title = "Ошибка: " + ex.Message + (ex.InnerException?.Message is { } inner ? " — " + inner : "");
        }
    }

    private async void OnDeletePayment(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (DataContext is not WorkOrderPaymentsViewModel vm) return;
            if ((sender as Button)?.CommandParameter is not PaymentRow row) return;
            await vm.DeletePaymentAsync(row);
        }
        catch (Exception ex)
        {
            Title = "Ошибка: " + ex.Message;
        }
    }

    private void OnClose(object? sender, RoutedEventArgs e) => Close();
}
