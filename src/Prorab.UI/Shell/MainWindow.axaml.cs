using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Prorab.UI.Shell;

public partial class MainWindow : Window
{
    private static readonly string MenuFoldPath = "M15.41 7.41L14 6l-6 6 6 6 1.41-1.41L10.83 12z";
    private static readonly string MenuUnfoldPath = "M8.59 16.59L10 18l6-6-6-6-1.41 1.41L13.17 12z";

    private bool _collapsed;

    public MainWindow()
    {
        InitializeComponent();
        UpdateToggleIcon();
    }

    private void HandleToggleSidebarClick(object? sender, RoutedEventArgs e)
    {
        _collapsed = !_collapsed;
        Sidebar.Width = _collapsed ? 52 : 220;
        NavList.Classes.Set("collapsed", _collapsed);
        AppTitle.IsVisible = !_collapsed;
        UpdateToggleIcon();
    }

    private void UpdateToggleIcon()
    {
        ToggleButton.Content = new PathIcon
        {
            Width = 18,
            Height = 18,
            Data = Geometry.Parse(_collapsed ? MenuUnfoldPath : MenuFoldPath)
        };
    }
}
