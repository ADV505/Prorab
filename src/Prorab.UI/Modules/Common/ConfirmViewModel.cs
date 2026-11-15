using CommunityToolkit.Mvvm.ComponentModel;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Common;

public partial class ConfirmViewModel : ViewModelBase
{
    [ObservableProperty] private string _message = "";
}
