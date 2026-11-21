using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;
using Prorab.Infrastructure.Data;
using Prorab.UI.Modules.Workers;
using Prorab.UI.ViewModels;
using System.Collections.ObjectModel;

namespace Prorab.UI.Modules.Crews;

public partial class CrewsViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    private List<Crew> _all = [];
    private List<Crew> _archivedAll = [];
    private List<Worker> _workers = [];
    private List<Site> _sites = [];

    [ObservableProperty] private string _searchText = string.Empty;

    public ObservableCollection<Crew> Crews { get; } = [];
    public ObservableCollection<Crew> ArchivedCrews { get; } = [];

    public bool HasCrews => Crews.Count > 0;
    public bool HasArchivedCrews => ArchivedCrews.Count > 0;

    /// <summary>Работники для диалога бригады (активные).</summary>
    public List<Worker> WorkersForEdit => _workers;

    /// <summary>Объекты для диалога бригады (активные).</summary>
    public List<Site> SitesForEdit => _sites;

    public CrewsViewModel(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    public async Task LoadAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        _all = await db.Crews.AsNoTracking()
            .Include(c => c.Foreman)
            .Include(c => c.Members).ThenInclude(m => m.Worker)
            .Include(c => c.Sites).ThenInclude(cs => cs.Site)
            .Where(c => !c.IsArchived)
            .OrderBy(c => c.Name)
            .ToListAsync();

        _archivedAll = await db.Crews.AsNoTracking()
            .Include(c => c.Foreman)
            .Include(c => c.Members).ThenInclude(m => m.Worker)
            .Include(c => c.Sites).ThenInclude(cs => cs.Site)
            .Where(c => c.IsArchived)
            .OrderBy(c => c.Name)
            .ToListAsync();

        _workers = await db.Workers.AsNoTracking()
            .Where(w => !w.IsArchived)
            .OrderBy(w => w.FullName)
            .ToListAsync();

        _sites = await db.Sites.AsNoTracking()
            .Where(s => !s.IsArchived)
            .OrderBy(s => s.Name)
            .ToListAsync();

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var query = _all.AsEnumerable();
        var archived = _archivedAll.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var s = SearchText.Trim();
            query = query.Where(c => c.Name.Contains(s, StringComparison.OrdinalIgnoreCase)
                || (c.Foreman?.FullName.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false));
            archived = archived.Where(c => c.Name.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        Crews.Clear();
        foreach (var c in query) Crews.Add(c);

        ArchivedCrews.Clear();
        foreach (var c in archived) ArchivedCrews.Add(c);

        OnPropertyChanged(nameof(HasCrews));
        OnPropertyChanged(nameof(HasArchivedCrews));
    }

    public async Task AddAsync(Window owner)
    {
        var edit = new CrewEditViewModel(WorkersForEdit, SitesForEdit);
        await ShowEditAsync(edit, owner, null);
    }

    public async Task EditAsync(Crew crew, Window owner)
    {
        var edit = new CrewEditViewModel(WorkersForEdit, SitesForEdit, crew);
        await ShowEditAsync(edit, owner, crew);
    }

    private async Task ShowEditAsync(CrewEditViewModel edit, Window owner, Crew? existing)
    {
        var dlg = new CrewEditWindow(existing is null ? "Новая бригада" : "Изменить бригаду")
        {
            DataContext = edit
        };
        dlg.CreateWorkerAsync = () => CreateWorkerFromDialogAsync(dlg);

        if (await dlg.ShowDialog<bool?>(owner) != true) return;

        if (existing is null)
        {
            var crew = new Crew
            {
                Name = edit.Name.Trim(),
                ForemanWorkerId = edit.SelectedForeman?.Worker?.Id
            };

            await using var db = await _dbFactory.CreateDbContextAsync();
            db.Crews.Add(crew);
            await db.SaveChangesAsync();

            foreach (var workerId in edit.SelectedWorkerIds)
                db.CrewWorkers.Add(new CrewWorker { CrewId = crew.Id, WorkerId = workerId });

            foreach (var siteId in edit.SelectedSiteIds)
                db.CrewSites.Add(new CrewSite { CrewId = crew.Id, SiteId = siteId });

            await db.SaveChangesAsync();
        }
        else
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var crew = await db.Crews
                .Include(c => c.Members)
                .Include(c => c.Sites)
                .FirstAsync(c => c.Id == existing.Id);

            crew.Name = edit.Name.Trim();
            crew.ForemanWorkerId = edit.SelectedForeman?.Worker?.Id;

            crew.Members.Clear();
            foreach (var workerId in edit.SelectedWorkerIds)
                crew.Members.Add(new CrewWorker { CrewId = crew.Id, WorkerId = workerId });

            crew.Sites.Clear();
            foreach (var siteId in edit.SelectedSiteIds)
                crew.Sites.Add(new CrewSite { CrewId = crew.Id, SiteId = siteId });

            await db.SaveChangesAsync();
        }

        await LoadAsync();
    }

    /// <summary>Создаёт работника через диалог и возвращает его; null — отмена.</summary>
    public async Task<Worker?> CreateWorkerFromDialogAsync(Window owner)
    {
        var dlg = new WorkerEditWindow("Новый работник")
        {
            DataContext = new WorkerEditViewModel()
        };

        if (await dlg.ShowDialog<bool?>(owner) != true) return null;

        var edit = (WorkerEditViewModel)dlg.DataContext;
        var worker = new Worker
        {
            FullName = edit.FullName.Trim(),
            Phone = edit.Phone,
            // ... остальные поля — как в WorkersViewModel.AddAsync
        };

        await using var db = await _dbFactory.CreateDbContextAsync();
        db.Workers.Add(worker);
        await db.SaveChangesAsync();

        // держим кэш и следующий диалог актуальными
        _workers.Add(worker);
        _workers.Sort((a, b) => string.Compare(a.FullName, b.FullName, StringComparison.OrdinalIgnoreCase));

        return worker;
    }

    public async Task ArchiveAsync(Crew crew)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var entity = await db.Crews.FirstAsync(c => c.Id == crew.Id);
        entity.IsArchived = true;
        await db.SaveChangesAsync();
        await LoadAsync();
    }

    public async Task RestoreAsync(Crew crew)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var entity = await db.Crews.FirstAsync(c => c.Id == crew.Id);
        entity.IsArchived = false;
        await db.SaveChangesAsync();
        await LoadAsync();
    }
}
