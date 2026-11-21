using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;
using Prorab.Infrastructure.Data;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Sites;

public partial class SitesViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    /// <summary>Мастер-список активных объектов.</summary>
    private List<Site> _all = [];

    /// <summary>Мастер-список архивных объектов.</summary>
    private List<Site> _archivedAll = [];

    public ObservableCollection<Site> Sites { get; } = [];
    public ObservableCollection<Site> ArchivedSites { get; } = [];

    [ObservableProperty] private bool _isLoading;

    /// <summary>Поиск по названию и адресу, без учёта регистра.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSites))]
    [NotifyPropertyChangedFor(nameof(HasArchivedSites))]
    private string _searchText = string.Empty;

    public bool HasSites => Sites.Count > 0;
    public bool HasArchivedSites => ArchivedSites.Count > 0;

    public SitesViewModel(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    public async Task LoadAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        try
        {
            await using var db = await _factory.CreateDbContextAsync();
            await DbMigration.EnsureMigratedAsync(_factory);

            _all = await db.Sites
                .Where(s => !s.IsArchived)
                .OrderBy(s => s.Name)
                .ToListAsync();

            _archivedAll = await db.Sites
                .Where(s => s.IsArchived)
                .OrderBy(s => s.Name)
                .ToListAsync();

            ApplyFilter();
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task AddAsync(string name, string? address,
        DateTime? startDate, DateTime? endDate, string? contractNumber, decimal contractAmount)
    {
        name = name.Trim();
        if (name.Length == 0) return;

        var site = new Site
        {
            Name = name,
            Address = address,
            StartDate = startDate,
            EndDate = endDate,
            ContractNumber = contractNumber,
            ContractAmount = contractAmount,
        };

        await using var db = await _factory.CreateDbContextAsync();
        db.Sites.Add(site);
        await db.SaveChangesAsync();

        _all.Add(site);
        SortAll();
        ApplyFilter();
    }

    public async Task UpdateAsync(Site site, string name, string? address,
        DateTime? startDate, DateTime? endDate, string? contractNumber, decimal contractAmount)
    {
        name = name.Trim();
        if (name.Length == 0) return;

        site.Name = name;
        site.Address = address;
        site.StartDate = startDate;
        site.EndDate = endDate;
        site.ContractNumber = contractNumber;
        site.ContractAmount = contractAmount;

        await using var db = await _factory.CreateDbContextAsync();
        db.Sites.Update(site);
        await db.SaveChangesAsync();

        SortAll();
        ApplyFilter();
    }

    public async Task ArchiveAsync(Site site)
    {
        site.IsArchived = true;

        await using var db = await _factory.CreateDbContextAsync();
        db.Sites.Update(site);
        await db.SaveChangesAsync();

        _all.Remove(site);
        _archivedAll.Add(site);
        SortArchivedAll();
        ApplyFilter();
    }

    public async Task RestoreAsync(Site site)
    {
        site.IsArchived = false;

        await using var db = await _factory.CreateDbContextAsync();
        db.Sites.Update(site);
        await db.SaveChangesAsync();

        _archivedAll.Remove(site);
        _all.Add(site);
        SortAll();
        ApplyFilter();
    }

    private void SortAll() =>
        _all.Sort(static (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

    private void SortArchivedAll() =>
        _archivedAll.Sort(static (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

    /// <summary>Пересобирает обе видимые коллекции по текущему SearchText.</summary>
    private void ApplyFilter()
    {
        var filter = SearchText.Trim();
        Sites.Clear();
        ArchivedSites.Clear();

        foreach (var site in _all)
        {
            if (Matches(site, filter))
                Sites.Add(site);
        }
        foreach (var site in _archivedAll)
        {
            if (Matches(site, filter))
                ArchivedSites.Add(site);
        }
        OnPropertyChanged(nameof(HasSites));
        OnPropertyChanged(nameof(HasArchivedSites));
    }

    private static bool Matches(Site site, string filter) =>
        filter.Length == 0 ||
        site.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase) ||
        (site.Address?.StartsWith(filter, StringComparison.OrdinalIgnoreCase) ?? false);
}
