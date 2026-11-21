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
