using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AtomUI.Icons.AntDesign;
using Prorab.UI.Modules;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.ReferenceBooks;

public record ReferenceBookItem(string Id, string Title, string Description, AntDesignIconKind Icon);

public partial class ReferenceBooksViewModel : ViewModelBase
{
    private readonly IServiceProvider _services;
    private readonly IReadOnlyList<IReferenceModule> _references;
    private readonly Dictionary<string, ViewModelBase> _viewCache = new();

    [ObservableProperty] private ViewModelBase? _current;

    public ObservableCollection<ReferenceBookItem> Books { get; }

    public bool IsHome => Current is null;

    public ReferenceBooksViewModel(IEnumerable<IReferenceModule> references, IServiceProvider services)
    {
        _services = services;
        _references = references.ToList();
        Books = new(_references.Select(r => new ReferenceBookItem(r.Id, r.Title, r.Description, r.Icon)));
    }

    partial void OnCurrentChanged(ViewModelBase? value) => OnPropertyChanged(nameof(IsHome));

    [RelayCommand]
    private void Open(string id)
    {
        if (_viewCache.TryGetValue(id, out var cached))
        {
            Current = cached;
            return;
        }

        var module = _references.FirstOrDefault(r => r.Id == id);
        if (module is null) return;

        var viewModel = module.CreateViewModel();
        _viewCache[id] = viewModel;
        Current = viewModel;
    }

    [RelayCommand]
    private void Back() => Current = null;
}
