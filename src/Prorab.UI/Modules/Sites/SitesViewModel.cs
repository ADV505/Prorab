using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;
using Prorab.Infrastructure.Data;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Sites;

public partial class SitesViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public ObservableCollection<Site> Sites { get; } = new();

    [ObservableProperty] private bool _isLoading;


    public bool HasSites => Sites.Count > 0;

    public SitesViewModel(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    public async Task LoadAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        try
        {
            await using var db = await _factory.CreateDbContextAsync();
            await DbMigration.EnsureMigratedAsync(_factory);

            var sites = await db.Sites
                .Where(s => !s.IsArchived)
                .OrderBy(s => s.Name)
                .ToListAsync();

            Sites.Clear();
            foreach (var site in sites)
                Sites.Add(site);
        }
        finally
        {
            IsLoading = false;
        }
        OnPropertyChanged(nameof(HasSites));
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

        Sites.Add(site);
        OnPropertyChanged(nameof(HasSites));
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

        Sites.Remove(site);
        Sites.Add(site);
        OnPropertyChanged(nameof(HasSites));
    }


    public async Task ArchiveAsync(Site site)
    {
        site.IsArchived = true;

        await using var db = await _factory.CreateDbContextAsync();
        db.Sites.Update(site);
        await db.SaveChangesAsync();

        Sites.Remove(site);
        OnPropertyChanged(nameof(HasSites));
    }
}
