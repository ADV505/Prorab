using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using Prorab.UI.Navigation;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.WorkOrders;

public class WorkOrdersModule : INavigationModule
{
    public string Id => "workorders";
    public string Title => "Наряды";
    public int Order => 3; // подбери под порядок других модулей
    public MaterialIconKind Icon => MaterialIconKind.ClipboardText;

    public ViewModelBase CreateViewModel() =>
        AppServices.Services.GetRequiredService<WorkOrdersViewModel>();
}
