using Material.Icons;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Navigation;

public interface INavigationModule
{
    /// <summary>Уникальный идентификатор модуля — ключ пункта меню.</summary>
    string Id { get; }
    string Title { get; }
    int Order { get; }
    /// <summary>Иконка Material Design.</summary>
    MaterialIconKind Icon { get; }
    ViewModelBase CreateViewModel();
}
