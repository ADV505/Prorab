using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Prorab.Infrastructure.Data;
using Prorab.UI.Modules;
using Prorab.UI.Modules.Crews;
using Prorab.UI.Modules.Home;
using Prorab.UI.Modules.ReferenceBooks;
using Prorab.UI.Modules.Sites;
using Prorab.UI.Modules.WorkTypes;
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

        .AddSingleton<INavigationModule, ReferenceBooksModule>()
        .AddSingleton<IReferenceModule, CrewsModule>()
        .AddTransient<ReferenceBooksViewModel>()

        .AddSingleton<IReferenceModule, SitesModule>()
        .AddTransient<SitesViewModel>()

        .AddSingleton<IReferenceModule, WorkTypesModule>()
        .AddTransient<WorkTypesViewModel>()

        .AddTransient<CrewsViewModel>()
        .BuildServiceProvider();
}
