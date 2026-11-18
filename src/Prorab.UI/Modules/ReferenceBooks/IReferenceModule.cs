using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules;

/// <summary>Модуль-справочник: открывается плиткой из хаба, в меню не показывается.</summary>
public interface IReferenceModule
{
    string Id { get; }
    string Title { get; }
    string Description { get; }
    /// <summary>SVG-путь иконки (холст 24×24).</summary>
    string Icon { get; }
    ViewModelBase CreateViewModel();
}
