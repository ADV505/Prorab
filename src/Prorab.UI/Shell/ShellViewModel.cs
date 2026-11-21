using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Prorab.UI.Navigation;
using Prorab.UI.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Prorab.UI.Shell;

public partial class ShellViewModel : ViewModelBase
{
    public static ShellViewModel Instance { get; } = new();

    public ShellViewModel()
    {
        Selected = Modules.FirstOrDefault();
    }

    // --- Тайтлбар ---
    public string ViewSubTitle => "Прораб. Стройучёт";
    public string ViewTitle => Selected?.Title ?? "Главная";

    // --- Drawer ---
    [ObservableProperty]
    private bool _isDrawerOpen = true;

    [RelayCommand]
    private void ToggleDrawerOpen() => IsDrawerOpen = !IsDrawerOpen;

    // --- Навигация: модули приходят из DI, порядок — по Order ---
    public ObservableCollection<INavigationModule> Modules { get; } =
        new(AppServices.Services.GetServices<INavigationModule>().OrderBy(m => m.Order));

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ViewTitle))]
    private INavigationModule? _selected;

    [ObservableProperty]
    private object? _viewElement;

    private readonly Dictionary<string, ViewModelBase> _viewModelCache = new();

    partial void OnSelectedChanged(INavigationModule? value)
    {
        if (value is null) return;
        if (!_viewModelCache.TryGetValue(value.Id, out var vm))
            _viewModelCache[value.Id] = vm = value.CreateViewModel();
        ViewElement = vm;
    }

    // --- Тема ---
    [RelayCommand]
    private void ToggleTheme() =>
        Application.Current!.RequestedThemeVariant =
            Application.Current.ActualThemeVariant == ThemeVariant.Dark
                ? ThemeVariant.Light
                : ThemeVariant.Dark;
}
