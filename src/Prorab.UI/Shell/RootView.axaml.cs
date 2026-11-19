using System.ComponentModel;
using Avalonia.Controls;

namespace Prorab.UI.Shell;

public partial class RootView : UserControl
{
    public ShellViewModel? ViewModel
    {
        get => DataContext as ShellViewModel;
        set => DataContext = value;
    }

    public RootView()
    {
        InitializeComponent();
        ViewModel = ShellViewModel.Instance;
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        Drawer.SetCollapsed(!ViewModel.IsDrawerOpen);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ShellViewModel.IsDrawerOpen) && ViewModel is not null)
            Drawer.SetCollapsed(!ViewModel.IsDrawerOpen);
    }
}
