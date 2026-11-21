using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Prorab.Core.Models;
using Prorab.UI.Modules.Common;

namespace Prorab.UI.Modules.Workers;

public partial class WorkersView : UserControl
{
    public WorkersView()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private Window? OwnerWindow => TopLevel.GetTopLevel(this) as Window;

    private async void OnAdd(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not WorkersViewModel viewModel) return;
        if (OwnerWindow is not Window owner) return;

        var edit = new WorkerEditViewModel();
        var dialog = new WorkerEditWindow("Новый работник") { DataContext = edit };

        if (await dialog.ShowDialog<bool?>(owner) == true)
            await viewModel.AddAsync(edit.Build());
    }

    private async void OnEdit(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not WorkersViewModel viewModel) return;
        if ((sender as Button)?.CommandParameter is not Worker worker) return;
        if (OwnerWindow is not Window owner) return;

        var edit = new WorkerEditViewModel
        {
            FullName = worker.FullName,
            StatusIndex = (int)worker.Status,
            Phone = worker.Phone,
            DayRate = worker.DayRate is { } rate && rate > 0 ? rate.ToString("0.##") : null,
            PassportSeries = worker.PassportSeries,
            PassportNumber = worker.PassportNumber,
            PassportIssuedBy = worker.PassportIssuedBy,
            PassportIssueDate = worker.PassportIssueDate,
            RegistrationAddress = worker.RegistrationAddress,
            Snils = worker.Snils,
            Inn = worker.Inn,
        };
        var dialog = new WorkerEditWindow("Изменить работника") { DataContext = edit };

        if (await dialog.ShowDialog<bool?>(owner) == true)
        {
            var updated = edit.Build();

            worker.FullName = updated.FullName;
            worker.Status = updated.Status;
            worker.Phone = updated.Phone;
            worker.DayRate = updated.DayRate;
            worker.PassportSeries = updated.PassportSeries;
            worker.PassportNumber = updated.PassportNumber;
            worker.PassportIssuedBy = updated.PassportIssuedBy;
            worker.PassportIssueDate = updated.PassportIssueDate;
            worker.RegistrationAddress = updated.RegistrationAddress;
            worker.Snils = updated.Snils;
            worker.Inn = updated.Inn;

            await viewModel.UpdateAsync(worker);
        }
    }

    private async void OnArchive(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not WorkersViewModel viewModel) return;
        if ((sender as Button)?.CommandParameter is not Worker worker) return;
        if (OwnerWindow is not Window owner) return;

        var confirm = new ConfirmWindow($"Архивировать работника «{worker.FullName}»? Данные сохранятся.");
        if (await confirm.ShowDialog<bool?>(owner) == true)
            await viewModel.ArchiveAsync(worker);
    }

    private async void OnRestore(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not WorkersViewModel viewModel) return;
        if ((sender as Button)?.CommandParameter is not Worker worker) return;
        if (OwnerWindow is not Window owner) return;

        var confirm = new ConfirmWindow($"Вернуть работника «{worker.FullName}» из архива?");
        if (await confirm.ShowDialog<bool?>(owner) == true)
            await viewModel.RestoreAsync(worker);
    }
}
