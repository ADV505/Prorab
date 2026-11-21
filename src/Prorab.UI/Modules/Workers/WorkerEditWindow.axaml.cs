using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Prorab.UI.Modules.Workers;

public partial class WorkerEditWindow : Window
{
    public WorkerEditWindow() => InitializeComponent();

    public WorkerEditWindow(string title) : this()
    {
        Title = title;
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not WorkerEditViewModel vm) return;
        if (!vm.Validate()) return;
        Close(true);
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);

    // ---- Маска телефона: +7-(XXX)-XXX-XX-XX ----
    private void OnPhoneChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox box || DataContext is not WorkerEditViewModel) return;

        var formatted = FormatPhone(box.Text);
        if (box.Text != formatted)
        {
            box.Text = formatted;
            box.CaretIndex = box.Text.Length;
        }
    }

    // ---- Маска СНИЛС: XXX-XXX-XXX XX ----
    private void OnSnilsChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox box || DataContext is not WorkerEditViewModel) return;

        var formatted = FormatSnils(box.Text);
        if (box.Text != formatted)
        {
            box.Text = formatted;
            box.CaretIndex = box.Text.Length;
        }
    }

    // ---- ИНН: только цифры, максимум 12 ----
    private void OnInnChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox box || DataContext is not WorkerEditViewModel) return;

        var digits = new string((box.Text ?? "").Where(char.IsDigit).ToArray());

        if (digits.Length > 12) digits = digits[..12];

        if (box.Text != digits)
        {
            box.Text = digits;
            box.CaretIndex = box.Text.Length;
        }
    }

    private static string FormatPhone(string? input)
    {
        var digits = new string((input ?? "").Where(char.IsDigit).ToArray());
        if (digits.Length == 0) return string.Empty;

        // 8-ка и «голые» 10 цифр приводятся к +7
        if (digits[0] == '8')
            digits = "7" + digits[1..];
        else if (digits[0] != '7')
            digits = "7" + digits;
        if (digits.Length > 11)
            digits = digits[..11];

        var result = "+7";
        if (digits.Length == 1)
            return result + "-(";

        var rest = digits[1..];
        result += "-(";
        result += rest[..Math.Min(3, rest.Length)];
        if (rest.Length >= 3) result += ")";
        if (rest.Length > 3) result += "-" + rest.Substring(3, Math.Min(3, rest.Length - 3));
        if (rest.Length > 6) result += "-" + rest.Substring(6, Math.Min(2, rest.Length - 6));
        if (rest.Length > 8) result += "-" + rest.Substring(8, Math.Min(2, rest.Length - 8));
        return result;
    }

    private static string FormatSnils(string? input)
    {
        var digits = new string((input ?? "").Where(char.IsDigit).ToArray());
        if (digits.Length > 11) digits = digits[..11];

        var sb = new StringBuilder();
        if (digits.Length > 0) sb.Append(digits[..Math.Min(3, digits.Length)]);
        if (digits.Length > 3) sb.Append('-').Append(digits.Substring(3, Math.Min(3, digits.Length - 3)));
        if (digits.Length > 6) sb.Append('-').Append(digits.Substring(6, Math.Min(3, digits.Length - 6)));
        if (digits.Length > 9) sb.Append(' ').Append(digits.Substring(9, Math.Min(2, digits.Length - 9)));
        return sb.ToString();
    }
}
