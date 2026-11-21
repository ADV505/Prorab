using CommunityToolkit.Mvvm.ComponentModel;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.Sites;

public partial class NameEditViewModel : ViewModelBase
{
    [ObservableProperty] private string _name = "";
}
