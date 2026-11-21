using CommunityToolkit.Mvvm.ComponentModel;
using Prorab.Core.Models;

namespace Prorab.UI.Modules.Workers;

public partial class WorkerEditViewModel : ObservableObject
{
    private bool _showErrors;

    [ObservableProperty] private string _fullName = string.Empty;
    [ObservableProperty] private int _statusIndex; // 0 физлицо, 1 самозанятый, 2 ИП, 3 ООО
    [ObservableProperty] private string? _phone;
    [ObservableProperty] private string? _dayRate;
    [ObservableProperty] private string? _passportSeries;
    [ObservableProperty] private string? _passportNumber;
    [ObservableProperty] private string? _passportIssuedBy;
    [ObservableProperty] private DateTimeOffset? _passportIssueDate;
    [ObservableProperty] private string? _registrationAddress;
    [ObservableProperty] private string? _snils;
    [ObservableProperty] private string? _inn;
    [ObservableProperty] private string? _errorText;

    public WorkerEditViewModel() { }

    public WorkerEditViewModel(Worker worker) : this()
    {
        FullName = worker.FullName;
        StatusIndex = (int)worker.Status;
        Phone = worker.Phone;
        DayRate = worker.DayRate is { } rate && rate > 0 ? rate.ToString("0.##") : null;
        PassportSeries = worker.PassportSeries;
        PassportNumber = worker.PassportNumber;
        PassportIssuedBy = worker.PassportIssuedBy;
        PassportIssueDate = worker.PassportIssueDate;
        RegistrationAddress = worker.RegistrationAddress;
        Snils = worker.Snils;
        Inn = worker.Inn;
    }

    partial void OnFullNameChanged(string value) => OnPropertyChanged(nameof(FullNameError));
    partial void OnPhoneChanged(string? value) => OnPropertyChanged(nameof(PhoneError));
    partial void OnSnilsChanged(string? value) => OnPropertyChanged(nameof(SnilsError));
    partial void OnInnChanged(string? value) => OnPropertyChanged(nameof(InnError));

    partial void OnStatusIndexChanged(int value)
    {
        OnPropertyChanged(nameof(InnError));
        OnPropertyChanged(nameof(InnPlaceholder));
    }

    public bool IsPassportVisible => StatusIndex is 0 or 1;

    public string InnPlaceholder =>
        StatusIndex == 3 ? "ИНН (10 цифр)" : "ИНН (12 цифр)";

    // ---- Флаги ошибок для подсветки Classes.warning ----
    public bool FullNameError => _showErrors && FullName.Trim().Length == 0;
    public bool PhoneError => _showErrors && !IsPhoneValid;
    public bool SnilsError => _showErrors && !IsSnilsValid;
    public bool InnError => _showErrors && !IsInnValid;

    public bool HasErrors => FullNameError || PhoneError || SnilsError || InnError;

    // ---- Правила ----
    private bool IsPhoneValid
    {
        get
        {
            var digits = Digits(Phone);
            return digits.Length == 0 || (digits.Length == 11 && digits[0] == '7');
        }
    }

    private bool IsSnilsValid
    {
        get
        {
            var digits = Digits(Snils);
            return digits.Length == 0 || digits.Length == 11;
        }
    }

    private bool IsInnValid
    {
        get
        {
            var digits = Digits(Inn);
            return digits.Length == 0 || digits.Length == (StatusIndex == 3 ? 10 : 12);
        }
    }

    public bool Validate()
    {
        _showErrors = true;
        RefreshErrors();
        return !HasErrors;
    }

    private void RefreshErrors()
    {
        OnPropertyChanged(nameof(FullNameError));
        OnPropertyChanged(nameof(PhoneError));
        OnPropertyChanged(nameof(SnilsError));
        OnPropertyChanged(nameof(InnError));
        ErrorText = HasErrors ? "Исправьте поля, выделенные красным." : null;
    }

    public Worker Build() => new()
    {
        FullName = FullName.Trim(),
        Status = (WorkerStatus)StatusIndex,
        Phone = NullIfEmpty(Phone),
        DayRate = decimal.TryParse(DayRate?.Replace('.', ','), out var rate) ? rate : null,
        PassportSeries = NullIfEmpty(PassportSeries),
        PassportNumber = NullIfEmpty(PassportNumber),
        PassportIssuedBy = NullIfEmpty(PassportIssuedBy),
        PassportIssueDate = PassportIssueDate?.DateTime,
        RegistrationAddress = NullIfEmpty(RegistrationAddress),
        Snils = NullIfEmpty(Snils),
        Inn = Digits(Inn).Length > 0 ? Digits(Inn) : null,
    };

    private static string Digits(string? value) =>
        new string((value ?? "").Where(char.IsDigit).ToArray());

    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
