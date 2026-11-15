using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Prorab.UI.Modules.ReferenceBooks;

public partial class ReferenceBooksView : UserControl
{
    public ReferenceBooksView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
