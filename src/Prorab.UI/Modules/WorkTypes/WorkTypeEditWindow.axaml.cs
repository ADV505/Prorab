
using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;

namespace Prorab.UI.Modules.WorkTypes;

public partial class WorkTypeEditWindow : Window
{
    public WorkTypeEditWindow() => InitializeComponent();
    public WorkTypeEditWindow(string title) : this() => Title = title;


    private void OnSave(object? sender, RoutedEventArgs e) => Close(true);
    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}
