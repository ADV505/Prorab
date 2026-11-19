using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Prorab.UI.Shell;

public partial class DrawerView : UserControl
{
    public DrawerView() => InitializeComponent();

    // Переключение в свёрнутый режим, как в Actipro: drawer сам знает, как себя рисовать.
    public void SetCollapsed(bool collapsed)
        => NavList.Classes.Set("collapsed", collapsed);
}
