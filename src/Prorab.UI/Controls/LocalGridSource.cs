// Prorab.UI/Controls/LocalGridSource.cs
using AtomUI.Controls.Data;
using AtomUI.Desktop.Controls;
using System.Collections.ObjectModel;

namespace Prorab.UI.Controls;

public sealed class LocalGridSource<T> : IDisposable
{
    public LocalGridSource(IEnumerable<T> rows, DataGridLocalSourceDescriptor<T> descriptor)
    {
        Rows = new ObservableCollection<T>(rows);
        Source = DataGridLocalSource.Create(Rows, descriptor);
    }

    /// <summary>Живая коллекция строк — в неё и пишут команды ViewModel.</summary>
    public ObservableCollection<T> Rows { get; }

    public DataGridLocalSource<T> Source { get; }

    public void Dispose() => Source.Dispose();
}
