using CommunityToolkit.Mvvm.ComponentModel;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Sites;

public partial class SiteEditViewModel : ViewModelBase
{
    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string? _address;
}
