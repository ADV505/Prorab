using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using Prorab.UI.Navigation;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.ReferenceBooks;

public class ReferenceBooksModule(IServiceProvider services) : INavigationModule
{
    public string Id => "reference-books";
    public string Title => "Справочники";
    public MaterialIconKind Icon => MaterialIconKind.Database;
    public int Order => 10;

    public ViewModelBase CreateViewModel()
        => services.GetRequiredService<ReferenceBooksViewModel>();
}
