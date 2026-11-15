using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;
using Prorab.Infrastructure.Data;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.WorkTypes;

public partial class WorkTypesViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public ObservableCollection<WorkType> WorkTypes { get; } = new();

    [ObservableProperty] private bool _isLoading;

    public bool HasWorkTypes => WorkTypes.Count > 0;

    public WorkTypesViewModel(IDbContextFactory<AppDbContext> factory) => _factory = factory;

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
