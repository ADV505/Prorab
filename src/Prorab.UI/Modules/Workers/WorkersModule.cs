using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using Prorab.UI.Modules;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Workers;

public class WorkersModule(IServiceProvider services) : IReferenceModule
{
    public string Id => "workers";
    public string Title => "Работники";
    public string Description => "Люди, самозанятые, ИП и организации";
    public MaterialIconKind Icon => MaterialIconKind.AccountGroup;

    public ViewModelBase CreateViewModel()
    {
        var viewModel = services.GetRequiredService<WorkersViewModel>();
        _ = viewModel.LoadAsync();
        return viewModel;
    }
}
