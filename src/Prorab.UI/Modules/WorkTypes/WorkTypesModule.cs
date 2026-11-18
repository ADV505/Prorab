using Microsoft.Extensions.DependencyInjection;
using Prorab.UI.Modules;
using Prorab.UI.Navigation;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.WorkTypes;

public class WorkTypesModule(IServiceProvider services) : IReferenceModule
{
    public string Id => "work-types";
    public string Title => "Виды работ";
    public string Description => "Список работ с единицами измерения";
    public string Icon => Icons.WorkTypes;

    public ViewModelBase CreateViewModel()
    {
        var viewModel = services.GetRequiredService<WorkTypesViewModel>();
        _ = viewModel.LoadAsync();
        return viewModel;
    }
}
