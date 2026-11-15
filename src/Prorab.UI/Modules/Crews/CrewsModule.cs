using AtomUI.Icons.AntDesign;
using Microsoft.Extensions.DependencyInjection;
using Prorab.UI.Modules;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Crews;

public class CrewsModule(IServiceProvider services) : IReferenceModule
{
    public string Id => "crews";
    public string Title => "Бригады";
    public string Description => "Состав бригад, договора";
    public AntDesignIconKind Icon => AntDesignIconKind.ContactsTwoTone;

    public ViewModelBase CreateViewModel()
    {
        var viewModel = services.GetRequiredService<CrewsViewModel>();
        _ = viewModel.LoadAsync();
        return viewModel;
    }
}
