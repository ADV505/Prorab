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

    /// <summary>Источник для atom:DataGrid — биндится в XAML к ItemsSource.</summary>
    public DataGridLocalSource<WorkType> WorkTypesSource => _gridSource.Source;

    /// <summary>Живая коллекция строк: в неё пишут Add/Update/Archive — грид обновится сам.</summary>
    public ObservableCollection<WorkType> WorkTypes => _gridSource.Rows;

    [ObservableProperty] private bool _isLoading;

    public bool HasWorkTypes => WorkTypes.Count > 0;

    public WorkTypesViewModel(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
        // Источник создаётся один раз поверх пустой коллекции.
        // Данные зальются в LoadAsync — биндинг при этом не меняется.
        _gridSource = new LocalGridSource<WorkType>([], WorkTypeDescriptors.Base);
    }

    public async Task LoadAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        try
        {
            await using var db = await _factory.CreateDbContextAsync();
            await DbMigration.EnsureMigratedAsync(_factory);

            var workTypes = await db.WorkTypes
                .Where(w => !w.IsArchived)
                .OrderBy(w => w.Name)
                .ToListAsync();

            WorkTypes.Clear();
            foreach (var workType in workTypes)
                WorkTypes.Add(workType);
        }
        finally
        {
            IsLoading = false;
        }
        OnPropertyChanged(nameof(HasWorkTypes));
    }

    public async Task AddAsync(string name, string? unit, decimal basePrice)
    {
        name = name.Trim();
        if (name.Length == 0) return;

        var workType = new WorkType { Name = name, Unit = unit, BasePrice = basePrice };

        await using var db = await _factory.CreateDbContextAsync();
        db.WorkTypes.Add(workType);
        await db.SaveChangesAsync();

        WorkTypes.Add(workType);
        OnPropertyChanged(nameof(HasWorkTypes));
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

        WorkTypes.Remove(workType);
        WorkTypes.Add(workType);
        OnPropertyChanged(nameof(HasWorkTypes));
    }

    public async Task ArchiveAsync(WorkType workType)
    {
        workType.IsArchived = true;

        await using var db = await _factory.CreateDbContextAsync();
        db.WorkTypes.Update(workType);
        await db.SaveChangesAsync();

        WorkTypes.Remove(workType);
        OnPropertyChanged(nameof(HasWorkTypes));
    }
}
