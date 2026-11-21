using Avalonia.Controls;
using Avalonia.Interactivity;
using Prorab.Core.Models;

namespace Prorab.UI.Modules.Crews;

public partial class CrewEditWindow : Window
{
    /// <summary>Создаёт работника через диалог и возвращает его; null — отмена.</summary>
    public Func<Task<Worker?>>? CreateWorkerAsync { get; set; }

    public CrewEditWindow() => InitializeComponent();

    public CrewEditWindow(string title) : this() => Title = title;

    private async void OnAddWorker(object? sender, RoutedEventArgs e)
    {
        if (CreateWorkerAsync is null || DataContext is not CrewEditViewModel vm) return;

        var worker = await CreateWorkerAsync();
        if (worker is null) return;

        vm.AddWorker(worker);
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not CrewEditViewModel vm) return;
        if (!vm.Validate()) return;
        Close(true);
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}
