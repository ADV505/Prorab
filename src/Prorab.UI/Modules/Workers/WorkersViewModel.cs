using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;
using Prorab.Infrastructure.Data;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Workers;

public partial class WorkersViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    private List<Worker> _all = [];
    private List<Worker> _archivedAll = [];

    public ObservableCollection<Worker> Workers { get; } = [];
    public ObservableCollection<Worker> ArchivedWorkers { get; } = [];

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasWorkers))]
    [NotifyPropertyChangedFor(nameof(HasArchivedWorkers))]
    private string _searchText = string.Empty;

    public bool HasWorkers => Workers.Count > 0;
    public bool HasArchivedWorkers => ArchivedWorkers.Count > 0;

    public WorkersViewModel(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    public async Task LoadAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        try
        {
            await using var db = await _factory.CreateDbContextAsync();
            await DbMigration.EnsureMigratedAsync(_factory);

            _all = await db.Workers
                .Where(w => !w.IsArchived)
                .OrderBy(w => w.FullName)
                .ToListAsync();

            _archivedAll = await db.Workers
                .Where(w => w.IsArchived)
                .OrderBy(w => w.FullName)
                .ToListAsync();

            ApplyFilter();
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task AddAsync(Worker worker)
    {
        worker.FullName = worker.FullName.Trim();
        if (worker.FullName.Length == 0) return;

        await using var db = await _factory.CreateDbContextAsync();
        db.Workers.Add(worker);
        await db.SaveChangesAsync();

        _all.Add(worker);
        SortAll();
        ApplyFilter();
    }

    public async Task UpdateAsync(Worker worker)
    {
        worker.FullName = worker.FullName.Trim();
        if (worker.FullName.Length == 0) return;

        await using var db = await _factory.CreateDbContextAsync();
        db.Workers.Update(worker);
        await db.SaveChangesAsync();

        SortAll();
        ApplyFilter();
    }

    public async Task ArchiveAsync(Worker worker)
    {
        worker.IsArchived = true;
        await using var db = await _factory.CreateDbContextAsync();
        db.Workers.Update(worker);
        await db.SaveChangesAsync();

        _all.Remove(worker);
        _archivedAll.Add(worker);
        SortArchivedAll();
        ApplyFilter();
    }

    public async Task RestoreAsync(Worker worker)
    {
        worker.IsArchived = false;
        await using var db = await _factory.CreateDbContextAsync();
        db.Workers.Update(worker);
        await db.SaveChangesAsync();

        _archivedAll.Remove(worker);
        _all.Add(worker);
        SortAll();
        ApplyFilter();
    }

    private void SortAll() =>
        _all.Sort(static (a, b) => string.Compare(a.FullName, b.FullName, StringComparison.Ordinal));

    private void SortArchivedAll() =>
        _archivedAll.Sort(static (a, b) => string.Compare(a.FullName, b.FullName, StringComparison.Ordinal));

    private void ApplyFilter()
    {
        var filter = SearchText.Trim();
        Workers.Clear();
        ArchivedWorkers.Clear();

        foreach (var w in _all)
            if (Matches(w, filter)) Workers.Add(w);
        foreach (var w in _archivedAll)
            if (Matches(w, filter)) ArchivedWorkers.Add(w);

        OnPropertyChanged(nameof(HasWorkers));
        OnPropertyChanged(nameof(HasArchivedWorkers));
    }

    private static bool Matches(Worker w, string filter) =>
        filter.Length == 0 ||
        w.FullName.StartsWith(filter, StringComparison.OrdinalIgnoreCase) ||
        (w.Phone?.StartsWith(filter, StringComparison.OrdinalIgnoreCase) ?? false);
}
