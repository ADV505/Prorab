using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Prorab.UI.Modules.Sites;

public partial class SiteEditWindow : Window
{
    public SiteEditWindow() => InitializeComponent();
    public SiteEditWindow(string title) : this() => Title = title;

    private void OnSave(object? sender, RoutedEventArgs e) => Close(true);
    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}
