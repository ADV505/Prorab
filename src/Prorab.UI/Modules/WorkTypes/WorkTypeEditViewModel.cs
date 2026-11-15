using CommunityToolkit.Mvvm.ComponentModel;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.WorkTypes;

public partial class WorkTypeEditViewModel : ViewModelBase
{
    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string? _unit;
}
