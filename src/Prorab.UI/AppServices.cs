using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Prorab.Infrastructure.Data;
using Prorab.UI.Modules;
using Prorab.UI.Modules.Crews;
using Prorab.UI.Modules.Home;
using Prorab.UI.Modules.ReferenceBooks;
using Prorab.UI.Modules.Sites;
using Prorab.UI.Modules.Workers;
using Prorab.UI.Modules.WorkOrders;
using Prorab.UI.Modules.WorkTypes;
using Prorab.UI.Navigation;
using Prorab.UI.Services;
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

        .AddSingleton<IPdfReportService, PdfReportService>()
        .AddSingleton<IFileSaveService>(_ =>
        new FileSaveService(() => App.MainWindow))
        // 1. Модуль — без этой строки меню пустое
        .AddSingleton<INavigationModule, HomeModule>()
        // 2. ViewModel модуля — без неё CreateViewModel упадёт
        .AddTransient<HomeViewModel>()
        

        .AddSingleton<INavigationModule, ReferenceBooksModule>()
        .AddSingleton<IReferenceModule, CrewsModule>()
        .AddTransient<ReferenceBooksViewModel>()

        .AddSingleton<IReferenceModule, SitesModule>()
        .AddTransient<SitesViewModel>()
        .AddTransient<SiteStructureViewModel>()

        .AddSingleton<IReferenceModule, WorkersModule>()
        .AddSingleton<WorkersViewModel>()


        .AddSingleton<IReferenceModule, WorkTypesModule>()
        .AddTransient<WorkTypesViewModel>()

        .AddTransient<CrewsViewModel>()

        .AddSingleton<INavigationModule, WorkOrdersModule>()
        .AddTransient<WorkOrderEditViewModel>()
        .AddTransient<WorkOrdersViewModel>()
        .AddTransient<WorkOrderPaymentsViewModel>()



        .BuildServiceProvider();
}
