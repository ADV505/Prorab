using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;
using Prorab.Infrastructure.Data;
using Prorab.UI.ViewModels;
using System.Collections.ObjectModel;

namespace Prorab.UI.Modules.WorkTypes;

public partial class WorkTypesViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    /// <summary>Полный список активных записей (без учёта фильтра).</summary>
    private List<WorkType> _all = [];

    /// <summary>Источник для DataGrid — отфильтрованный срез _all.</summary>
    public ObservableCollection<WorkType> WorkTypes { get; } = [];

    [ObservableProperty] private bool _isLoading;

    /// <summary>Поисковая строка. Фильтрация — по первым буквам названия, без учёта регистра.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasWorkTypes))]
    private string _searchText = string.Empty;

    public bool HasWorkTypes => WorkTypes.Count > 0;

    public WorkTypesViewModel(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    public async Task LoadAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        try
        {
            await using var db = await _factory.CreateDbContextAsync();
            await DbMigration.EnsureMigratedAsync(_factory);

            _all = await db.WorkTypes
                .Where(w => !w.IsArchived)
                .OrderBy(w => w.Name)
                .ToListAsync();

            ApplyFilter();
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task AddAsync(string name, string? unit, decimal basePrice)
    {
        name = name.Trim();
        if (name.Length == 0) return;

        var workType = new WorkType { Name = name, Unit = unit, BasePrice = basePrice };

        await using var db = await _factory.CreateDbContextAsync();
        db.WorkTypes.Add(workType);
        await db.SaveChangesAsync();

        // Вставляем на алфавитное место и в мастер-список, и в видимую коллекцию.
        _all.Add(workType);
        _all.Sort(static (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
        ApplyFilter();
    }

    public async Task UpdateAsync(WorkType workType, string name, string? unit, decimal basePrice)
    {
        name = name.Trim();
        if (name.Length == 0) return;

        workType.Name = name;
        workType.Unit = unit;
        workType.BasePrice = basePrice;

        await using var db = await _factory.CreateDbContextAsync();
        db.WorkTypes.Update(workType);
        await db.SaveChangesAsync();

        // Пересортировка: после правки названия строка должна встать на своё место.
        _all.Sort(static (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
        ApplyFilter();
    }

    public async Task ArchiveAsync(WorkType workType)
    {
        workType.IsArchived = true;

        await using var db = await _factory.CreateDbContextAsync();
        db.WorkTypes.Update(workType);
        await db.SaveChangesAsync();

        _all.Remove(workType);
        ApplyFilter();
    }

    /// <summary>Пересобирает видимую коллекцию по текущему SearchText.</summary>
    private void ApplyFilter()
    {
        var filter = SearchText.Trim();
        WorkTypes.Clear();

        foreach (var workType in _all)
        {
            if (filter.Length == 0 ||
                workType.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            {
                WorkTypes.Add(workType);
            }
        }
        OnPropertyChanged(nameof(HasWorkTypes));
    }
}
