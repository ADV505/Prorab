using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Prorab.Core.Models;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Crews;

/// <summary>Обёртка работника для чекбокса состава.</summary>
public partial class SelectableWorker : ViewModelBase
{
    public Worker Item { get; }

    [ObservableProperty] private bool _isSelected;

    public SelectableWorker(Worker item, bool isSelected = false)
    {
        Item = item;
        _isSelected = isSelected;
    }

    public string Display => Item.FullName;
}

/// <summary>Обёртка объекта для чекбокса привязок.</summary>
public partial class SelectableSite : ViewModelBase
{
    public Site Item { get; }

    [ObservableProperty] private bool _isSelected;

    public SelectableSite(Site item, bool isSelected = false)
    {
        Item = item;
        _isSelected = isSelected;
    }

    public string Display => Item.Name;
}

/// <summary>Вариант комбобокса бригадира: «Не назначен» + работники.</summary>
public class ForemanOption
{
    public Worker? Worker { get; init; }
    public string Name => Worker?.FullName ?? "Не назначен";
}

public partial class CrewEditViewModel : ViewModelBase
{
    private bool _showErrors;

    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private ForemanOption? _selectedForeman;
    [ObservableProperty] private string? _errorText;

    public ObservableCollection<ForemanOption> ForemanOptions { get; } = [];
    public ObservableCollection<SelectableWorker> WorkerOptions { get; } = [];
    public ObservableCollection<SelectableSite> SiteOptions { get; } = [];

    public bool HasNoWorkers => WorkerOptions.Count == 0;
    public bool HasNoSites => SiteOptions.Count == 0;

    public CrewEditViewModel(List<Worker> workers, List<Site> sites, Crew? crew = null)
    {
        ForemanOptions.Add(new ForemanOption());
        foreach (var w in workers)
        {
            ForemanOptions.Add(new ForemanOption { Worker = w });

            WorkerOptions.Add(new SelectableWorker(w,
                crew?.Members.Any(m => m.WorkerId == w.Id) == true));
        }

        foreach (var s in sites)
            SiteOptions.Add(new SelectableSite(s,
                crew?.Sites.Any(cs => cs.SiteId == s.Id) == true));

        SelectedForeman = crew is null
            ? ForemanOptions[0]
            : ForemanOptions.FirstOrDefault(o => o.Worker?.Id == crew.ForemanWorkerId)
              ?? ForemanOptions[0];

        if (crew is not null)
            Name = crew.Name;
    }

    /// <summary>Добавляет только что созданного работника и сразу выбирает его бригадиром.</summary>
    public void AddWorker(Worker worker)
    {
        if (ForemanOptions.Any(o => o.Worker?.Id == worker.Id)) return;

        var option = new ForemanOption { Worker = worker };
        ForemanOptions.Add(option);
        SelectedForeman = option;

        if (WorkerOptions.All(o => o.Item.Id != worker.Id))
            WorkerOptions.Add(new SelectableWorker(worker));
    }

    partial void OnNameChanged(string value) => OnPropertyChanged(nameof(NameError));

    public bool NameError => _showErrors && Name.Trim().Length == 0;
    public bool HasErrors => NameError;

    public bool Validate()
    {
        _showErrors = true;
        OnPropertyChanged(nameof(NameError));
        ErrorText = HasErrors ? "Укажите название бригады." : null;
        return !HasErrors;
    }

    public List<int> SelectedWorkerIds =>
        WorkerOptions.Where(o => o.IsSelected).Select(o => o.Item.Id).ToList();

    public List<int> SelectedSiteIds =>
        SiteOptions.Where(o => o.IsSelected).Select(o => o.Item.Id).ToList();
}
