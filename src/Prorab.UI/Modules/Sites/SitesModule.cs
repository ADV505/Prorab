using AtomUI.Icons.AntDesign;
using Microsoft.Extensions.DependencyInjection;
using Prorab.UI.Modules;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Sites;

public class SitesModule(IServiceProvider services) : IReferenceModule
{
    public string Id => "sites";
    public string Title => "Объекты";
    public string Description => "Стройплощадки, адреса, заказчики";
    public AntDesignIconKind Icon => AntDesignIconKind.EnvironmentTwoTone;

    public ViewModelBase CreateViewModel()
    {
        var viewModel = services.GetRequiredService<SitesViewModel>();
        _ = viewModel.LoadAsync();
        return viewModel;
    }
}
