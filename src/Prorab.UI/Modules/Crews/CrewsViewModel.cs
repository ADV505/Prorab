using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;
using Prorab.Infrastructure.Data;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Crews;

public partial class CrewsViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public ObservableCollection<Crew> Crews { get; } = new();

    [ObservableProperty] private bool _isLoading;

    public CrewsViewModel(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    public bool HasCrews => Crews.Count > 0;

    public async Task LoadAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        try
        {
            await using var db = await _factory.CreateDbContextAsync();           
            await DbMigration.EnsureMigratedAsync(_factory);

            var crews = await db.Crews
                .Where(c => !c.IsArchived)
                .OrderBy(c => c.Name)
                .ToListAsync();

            Crews.Clear();
            foreach (var crew in crews)
                Crews.Add(crew);
        }
        finally
        {
            IsLoading = false;
        }
    }


    public async Task AddAsync(string name, string? foremanName, string? phone)
    {
        name = name.Trim();
        if (name.Length == 0) return;

        var crew = new Crew { Name = name, ForemanName = foremanName, Phone = phone };

        await using var db = await _factory.CreateDbContextAsync();
        db.Crews.Add(crew);
        await db.SaveChangesAsync();

        Crews.Add(crew);
        OnPropertyChanged(nameof(HasCrews));
    }

    public async Task UpdateAsync(Crew crew, string name, string? foremanName, string? phone)
    {
        name = name.Trim();
        if (name.Length == 0) return;

        crew.Name = name;
        crew.ForemanName = foremanName;
        crew.Phone = phone;

        await using var db = await _factory.CreateDbContextAsync();
        db.Crews.Update(crew);
        await db.SaveChangesAsync();

        // Обновим позицию сортировки, если имя поменялось
        var index = Crews.IndexOf(crew);
        if (index >= 0)
        {
            Crews.Remove(crew);
            var newIndex = Crews.IndexOf(Crews.FirstOrDefault(c => string.Compare(c.Name, name, StringComparison.Ordinal) > 0) ?? Crews[^1]) + 1;
            if (newIndex > Crews.Count) newIndex = Crews.Count;
            Crews.Insert(newIndex, crew);
        }
    }

    public async Task ArchiveAsync(Crew crew)
    {
        crew.IsArchived = true;

        await using var db = await _factory.CreateDbContextAsync();
        db.Crews.Update(crew);
        await db.SaveChangesAsync();

        Crews.Remove(crew);
        OnPropertyChanged(nameof(HasCrews));
    }


}
