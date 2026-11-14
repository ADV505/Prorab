using AtomUI.Icons.AntDesign;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Navigation;

public interface INavigationModule
{
    /// <summary>Уникальный идентификатор модуля — ключ пункта меню NavMenu.</summary>
    string Id {  get; }
    string Title { get; }
    int Order { get; }
    /// <summary>Иконка пункта меню.</summary>
    AntDesignIconKind Icon { get; }
    ViewModelBase CreateViewModel();
}
