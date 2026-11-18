using Microsoft.Extensions.DependencyInjection;
using Prorab.UI.Navigation;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Home;

public class HomeModule(IServiceProvider services) : INavigationModule
{
    public string Id => "home";
    public string Title => "Главная";
    public string Icon => Icons.Home;
    public int Order => 0;

    public ViewModelBase CreateViewModel()
    {
        var vm = services.GetRequiredService<HomeViewModel>();
        _ = RunLoadAsync(vm); // не ждём: VM возвращается сразу, цифры подтянутся за долю секунды
        return vm;
    }

    private static async Task RunLoadAsync(HomeViewModel vm)
    {
        try
        {
            await vm.LoadAsync();
        }
        catch
        {
            // Пока молча: когда появятся логи или toast — сюда их и воткнём.
        }
    }
}
