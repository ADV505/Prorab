using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Prorab.Core.Models;
using Prorab.UI.Modules.Common;

namespace Prorab.UI.Modules.Sites;

public partial class SitesView : UserControl
{
    public SitesView()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private Window? OwnerWindow => TopLevel.GetTopLevel(this) as Window;

    private async void OnAdd(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SitesViewModel viewModel) return;
        if (OwnerWindow is not Window owner) return;

        var edit = new SiteEditViewModel();
        var dialog = new SiteEditWindow("Новый объект") { DataContext = edit };

        if (await dialog.ShowDialog<bool?>(owner) == true)
        {
            decimal.TryParse(edit.ContractAmount?.Replace('.', ','), out var amount);
            await viewModel.AddAsync(edit.Name, edit.Address,
                edit.StartDate?.DateTime, edit.EndDate?.DateTime, amount);
        }

    }

    private async void OnEdit(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SitesViewModel viewModel) return;
        if ((sender as Button)?.CommandParameter is not Site site) return;
        if (OwnerWindow is not Window owner) return;

        var edit = new SiteEditViewModel
        {
            Name = site.Name,
            Address = site.Address,
            StartDate = site.StartDate,
            EndDate = site.EndDate,
            ContractAmount = site.ContractAmount == 0 ? "" : site.ContractAmount.ToString("0.##"),
        };
        var dialog = new SiteEditWindow("Изменить объект") { DataContext = edit };

        if (await dialog.ShowDialog<bool?>(owner) == true)
        {
            decimal.TryParse(edit.ContractAmount?.Replace('.', ','), out var amount);
            await viewModel.UpdateAsync(site, edit.Name, edit.Address,
                edit.StartDate?.DateTime, edit.EndDate?.DateTime, amount);
        }
    }

    private async void OnArchive(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SitesViewModel viewModel) return;
        if ((sender as Button)?.CommandParameter is not Site site) return;
        if (OwnerWindow is not Window owner) return;

        var confirm = new ConfirmWindow($"Архивировать объект «{site.Name}»? Он исчезнет из списка, но данные сохранятся.");
        if (await confirm.ShowDialog<bool?>(owner) == true)
            await viewModel.ArchiveAsync(site);
    }
}
