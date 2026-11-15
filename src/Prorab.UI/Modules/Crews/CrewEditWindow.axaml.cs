using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Prorab.UI.Modules.Crews;

public partial class CrewEditWindow : Window
{
    public CrewEditWindow() => InitializeComponent();

    private void OnSave(object? sender, RoutedEventArgs e) => Close(true);
    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}
