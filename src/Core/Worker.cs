namespace Prorab.Core.Models;

/// <summary>Правовой статус работника — определяет документы и расчёты.</summary>
public enum WorkerStatus
{
    Individual,     // физлицо по договору подряда
    SelfEmployed,   // самозанятый
    Entrepreneur,   // ИП
    Company,        // ООО (контрагент)
}

public class Worker
{
    public int Id { get; set; }

    /// <summary>ФИО полностью (для договора) или наименование для ИП/ООО.</summary>
    public string FullName { get; set; } = string.Empty;

    public WorkerStatus Status { get; set; }

    // ---- Паспортные данные (физлицо, самозанятый) ----
    public string? PassportSeries { get; set; }
    public string? PassportNumber { get; set; }
    public string? PassportIssuedBy { get; set; }
    public DateTime? PassportIssueDate { get; set; }
    public string? RegistrationAddress { get; set; }

    // ---- Общие ----
    public string? Snils { get; set; }
    public string? Inn { get; set; }        // обязателен самозанятым и ИП
    public string? Phone { get; set; }

    // ---- Договор подряда (оформление физлица) ----
    /// <summary>Номер договора подряда. Заполняется кнопкой «Сделать договор».</summary>
    public string? ContractNumber { get; set; }
    /// <summary>Дата договора подряда. Не null — значит официально устроен.</summary>
    public DateTime? ContractDate { get; set; }


    /// <summary>Дневная ставка — у каждого своя, для оплаты по дням.</summary>
    public decimal? DayRate { get; set; }

    public bool IsArchived { get; set; }

    public string StatusName => Status switch
    {
        WorkerStatus.Individual => "Физлицо",
        WorkerStatus.SelfEmployed => "Самозанятый",
        WorkerStatus.Entrepreneur => "ИП",
        WorkerStatus.Company => "ООО",
        _ => string.Empty,
    };

    /// <summary>Физлицо официально устроено: есть договор подряда.</summary>
    public bool IsHired =>
        Status == WorkerStatus.Individual && ContractDate is not null;

    /// <summary>Подпись статуса оформления для таблицы и списков.</summary>
    public string HireStatusName => Status switch
    {
        WorkerStatus.Individual when ContractDate is { } d =>
            $"Устроен · договор №{ContractNumber} от {d:dd.MM.yyyy}",
        WorkerStatus.Individual => "Не оформлен",
        _ => "", // самозанятые, ИП, ООО — не физлица по договору, отметка не нужна
    };

    // ---- Отображение для таблицы работника ----
    /// <summary>Кнопке «Сделать договор» нужен только неоформленный физлицо.</summary>
    public bool CanMakeContract =>
        Status == WorkerStatus.Individual && ContractDate is null;

    public string ContractText => HireStatusName.Length > 0 ? HireStatusName : "—";
    public string PhoneText => string.IsNullOrWhiteSpace(Phone) ? "—" : Phone!;
    public string RateText => DayRate is { } r && r > 0 ? r.ToString("0.##") : "—";
    public string SnilsText => string.IsNullOrWhiteSpace(Snils) ? "—" : Snils!;
    public string InnText => string.IsNullOrWhiteSpace(Inn) ? "—" : Inn!;

    public string PassportText
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(PassportSeries)) parts.Add(PassportSeries!);
            if (!string.IsNullOrWhiteSpace(PassportNumber)) parts.Add(PassportNumber!);
            return NeedsPassport && parts.Count > 0 ? string.Join(' ', parts) : "—";
        }
    }


    /// <summary>Паспорт нужен физлицам и самозанятым.</summary>
    public bool NeedsPassport =>
        Status is WorkerStatus.Individual or WorkerStatus.SelfEmployed;

    /// <summary>Строка «телефон · ставка» для карточки.</summary>
    public string Info
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(Phone))
                parts.Add(Phone);
            if (DayRate is { } rate && rate > 0)
                parts.Add($"{rate:N0} руб./день");
            return string.Join(" · ", parts);
        }
    }

    public bool HasInfo => Info.Length > 0;
}
