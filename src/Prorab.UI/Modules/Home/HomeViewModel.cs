using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Prorab.Infrastructure.Data;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Home;

public partial class HomeViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    /// <summary>Приветствие в шапке главной.</summary>
    public string Greeting => "Прораб. Стройучёт — добро пожаловать!";

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private int _activeSites;
    [ObservableProperty] private int _overdueSites;

    /// <summary>Сумма контрактов по действующим объектам, руб.</summary>
    [ObservableProperty] private decimal _activeAmount;

    /// <summary>Есть просроченные — карточка подкрашивается.</summary>
    public bool HasOverdue => OverdueSites > 0;

    public HomeViewModel(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    public async Task LoadAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        try
        {
            await using var db = await _factory.CreateDbContextAsync();
            await DbMigration.EnsureMigratedAsync(_factory);

            var today = DateTime.Today;

            // Действующий: срок не указан (работа без жёсткого дедлайна) или ещё не наступил.
            var active = db.Sites.Where(s =>
                !s.IsArchived && (s.EndDate == null || s.EndDate >= today));

            ActiveSites = await active.CountAsync();
            ActiveAmount = await active.SumAsync(s => (decimal?)s.ContractAmount) ?? 0m;

            // Срок вышел — для дашборда трактуем как «просроченные».
            OverdueSites = await db.Sites.CountAsync(s =>
                !s.IsArchived && s.EndDate != null && s.EndDate < today);

            OnPropertyChanged(nameof(HasOverdue));
        }
        finally
        {
            IsLoading = false;
        }
    }
}
