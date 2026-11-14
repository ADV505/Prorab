using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Window = AtomUI.Desktop.Controls.Window;

namespace Prorab.UI.Shell;

public partial class MainWindow : Window
{
    private ShellViewModel? ShellVm => DataContext as ShellViewModel;

    private readonly PathIcon _collapseIcon;
    private readonly PathIcon _expandIcon;

    public MainWindow()
    {
        InitializeComponent();

        _collapseIcon = CreateIcon(AntDesignIconKind.MenuFoldOutlined);
        _expandIcon = CreateIcon(AntDesignIconKind.MenuUnfoldOutlined);

        SidebarMenu.NavMenuItemClick += (_, args) =>
        {
            var id = args.NavMenuItem.ItemKey?.ToString();
            if (id is null)
            {
                return;
            }

            var vm = ShellVm;
            if (vm is null)
            {
                return;
            }

            vm.Selected = vm.Modules.FirstOrDefault(m => m.Id == id);
        };

        RebuildMenu();
        DataContextChanged += (_, _) => RebuildMenu();

        UpdateCollapseButtonPresentation();
        SidebarMenu.GetPropertyChangedObservable(NavMenu.IsInlineCollapsedProperty)
                   .Subscribe(_ => UpdateCollapseButtonPresentation());
    }

    private void RebuildMenu()
    {
        var vm = ShellVm;
        if (vm is null)
        {
            return;
        }

        SidebarMenu.Items.Clear();
        foreach (var module in vm.Modules)
        {
            SidebarMenu.Items.Add(new NavMenuNode
            {
                ItemKey = module.Id,
                Header = module.Title,
                Icon = CreateIcon(module.Icon),
            });
        }
    }

    private void HandleToggleCollapsedClick(object? sender, RoutedEventArgs e)
    {
        SidebarMenu.IsInlineCollapsed = !SidebarMenu.IsInlineCollapsed;
        UpdateCollapseButtonPresentation();
    }

    private void UpdateCollapseButtonPresentation()
    {
        CollapseButton.Icon = SidebarMenu.IsInlineCollapsed
            ? _expandIcon
            : _collapseIcon;
        //SidebarTitle.IsVisible = !SidebarMenu.IsInlineCollapsed;
    }

    private static PathIcon CreateIcon(AntDesignIconKind iconKind)
    {
        return (PathIcon)new AntDesignIconProvider(iconKind).ProvideValue(null!);
    }
}
