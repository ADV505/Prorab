using CommunityToolkit.Mvvm.ComponentModel;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Sites;

public partial class SiteEditViewModel : ViewModelBase
{
    /// <summary>
    /// Название объекта
    /// </summary>
    [ObservableProperty] private string _name = "";

    /// <summary>
    /// Адрес объекта
    /// </summary>
    [ObservableProperty] private string? _address;

    /// <summary>
    /// Начало действие договора
    /// </summary>
    [ObservableProperty] private DateTimeOffset? _startDate;

    /// <summary>
    /// Окончание действие договора
    /// </summary>
    [ObservableProperty] private DateTimeOffset? _endDate;

    /// <summary>
    /// Сумма контракта
    /// </summary>
    [ObservableProperty] private string? _contractAmount;

    /// <summary>
    /// Номер договора
    /// </summary>
    [ObservableProperty] private string? _contractNumber;

}
