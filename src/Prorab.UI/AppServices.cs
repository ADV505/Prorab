using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Prorab.Infrastructure.Data;
using Prorab.UI.Modules.Home;
using Prorab.UI.Navigation;
using Prorab.UI.Shell;
using Prorab.UI.ViewModels;

namespace Prorab.UI;

public static class AppServices
{
    public static IServiceProvider Services { get; } = new ServiceCollection()
        .AddSingleton<IDbContextFactory<AppDbContext>>(
            new AppDbContextFactory(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Prorab", "prorab.db")))
        // 1. Модуль — без этой строки меню пустое
        .AddSingleton<INavigationModule, HomeModule>()
        // 2. ViewModel модуля — без неё CreateViewModel упадёт
        .AddTransient<HomeViewModel>()
        // 3. Shell — без неё окно вообще не открылось бы
        .AddSingleton<ShellViewModel>()
        .BuildServiceProvider();
}
