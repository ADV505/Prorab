using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Prorab.UI.Modules.Home;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
