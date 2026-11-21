using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Prorab.UI.Modules.Sites;

public partial class NameEditWindow : Window
{
    public NameEditWindow() => InitializeComponent();
    public NameEditWindow(string title) : this() => Title = title;

    private void OnSave(object? sender, RoutedEventArgs e) => Close(true);
    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}
