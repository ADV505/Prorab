using Prorab.UI.ViewModels;

namespace Prorab.UI.Navigation;

public interface INavigationModule
{
    /// <summary>Уникальный идентификатор модуля — ключ пункта меню.</summary>
    string Id { get; }
    string Title { get; }
    int Order { get; }
    /// <summary>SVG-путь иконки (холст 24×24).</summary>
    string Icon { get; }
    ViewModelBase CreateViewModel();
}
