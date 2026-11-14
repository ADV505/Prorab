using CommunityToolkit.Mvvm.ComponentModel;
using Prorab.UI.Navigation;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Shell;

public partial class ShellViewModel : ViewModelBase
{
    public IReadOnlyList<INavigationModule> Modules { get; }

    [ObservableProperty] private INavigationModule? _selected;

    public ShellViewModel(IEnumerable<INavigationModule> modules)
    {
        Modules = modules.OrderBy(m => m.Order).ToList();
        Selected = Modules.FirstOrDefault();
    }

    partial void OnSelectedChanged(INavigationModule? value)
    {
        if (value is not null)
            Current = value.CreateViewModel();
    }

    [ObservableProperty] private ViewModelBase? _current;
}
