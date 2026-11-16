using AtomUI.Controls.Data;
using AtomUI.Desktop.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;
using Prorab.Infrastructure.Data;
using Prorab.UI.Controls;
using Prorab.UI.ViewModels;
using System.Collections.ObjectModel;

namespace Prorab.UI.Modules.WorkTypes;

/// <summary>Идентификаторы полей грида — нужны для сортировки по колонкам.</summary>
public static class WorkTypeFields
{
    public static DataGridFieldId Name { get; } = new("name");
    public static DataGridFieldId PriceInfo { get; } = new("price-info");
}

public static class WorkTypeDescriptors
{
    public static DataGridLocalSourceDescriptor<WorkType> Base { get; } =
        DataGridLocalSourceDescriptor
            .For<WorkType>(static row => DataGridRowKey.FromString(row.Id.ToString()))
            .Field(WorkTypeFields.Name, static row => row.Name, StringComparer.Ordinal)
            .Field(WorkTypeFields.PriceInfo, static row => row.PriceInfo, StringComparer.Ordinal);
}

public partial class WorkTypesViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly LocalGridSource<WorkType> _gridSource;

    /// <summary>Полный список активных записей (без учёта фильтра).</summary>
    private List<WorkType> _all = [];

    /// <summary>Источник для atom:DataGrid — биндится в XAML к ItemsSource.</summary>
    public DataGridLocalSource<WorkType> WorkTypesSource => _gridSource.Source;

    /// <summary>Живая коллекция строк — отфильтрованный срез _all.</summary>
    public ObservableCollection<WorkType> WorkTypes => _gridSource.Rows;

    [ObservableProperty] private bool _isLoading;

    /// <summary>Поисковая строка. Фильтрация — по первым буквам названия, без учёта регистра.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasWorkTypes))]
    private string _searchText = string.Empty;

    public bool HasWorkTypes => WorkTypes.Count > 0;

    public WorkTypesViewModel(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
        _gridSource = new LocalGridSource<WorkType>([], WorkTypeDescriptors.Base);
    }

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
        var rows = _gridSource.Rows;

        rows.Clear();
        foreach (var workType in _all)
        {
            if (filter.Length == 0 ||
                workType.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            {
                rows.Add(workType);
            }
        }
        OnPropertyChanged(nameof(HasWorkTypes));
    }
}
