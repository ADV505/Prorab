
using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;

namespace Prorab.UI.Modules.Common;

public partial class ConfirmWindow : Window
{
    public ConfirmWindow() => InitializeComponent();

    public ConfirmWindow(string message) : this()
    {
        DataContext = new ConfirmViewModel { Message = message };
    }

    private void OnOk(object? sender, RoutedEventArgs e) => Close(true);
    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}
