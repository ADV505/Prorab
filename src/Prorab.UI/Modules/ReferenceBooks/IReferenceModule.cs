using Material.Icons;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules;

/// <summary>Модуль-справочник: открывается плиткой из хаба, в меню не показывается.</summary>
public interface IReferenceModule
{
    string Id { get; }
    string Title { get; }
    string Description { get; }
    /// <summary>Иконка Material Design.</summary>
    MaterialIconKind Icon { get; }
    ViewModelBase CreateViewModel();
}
