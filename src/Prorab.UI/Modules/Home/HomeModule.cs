using AtomUI.Icons.AntDesign;
using Microsoft.Extensions.DependencyInjection;
using Prorab.UI.Navigation;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Home;

public class HomeModule(IServiceProvider services) : INavigationModule
{
    public string Id => "home";
    public string Title => "Главная";
    public AntDesignIconKind Icon => AntDesignIconKind.HomeOutlined;
    public int Order => 0;

    public ViewModelBase CreateViewModel()
        => services.GetRequiredService<HomeViewModel>();
}
