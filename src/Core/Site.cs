namespace Prorab.Core.Models;

public class Site
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }

    /// <summary>Дата начала работ на объекте.</summary>
    public DateTime? StartDate { get; set; }

    /// <summary>Планируемая дата окончания работ.</summary>
    public DateTime? EndDate { get; set; }

    /// <summary>Сумма контракта, руб.</summary>
    public decimal ContractAmount { get; set; }

    public bool IsArchived { get; set; }
    /// <summary>Номер договора с заказчиком.</summary>
    public string? ContractNumber { get; set; }

    /// <summary>Статус: «Действующий», пока дата окончания не наступила (или не указана); «Завершён» — когда она прошла.</summary>
    public string Status
    {
        get
        {
            if (EndDate is { } end && end.Date < DateTime.Today)
                return "Завершён";
            return "Действующий";
        }
    }

    /// <summary>Строка «сроки · сумма» для отображения в списке.</summary>
    public string Info
    {
        get
        {
            var parts = new List<string>();
            parts.Add(Status);
            if (StartDate is { } start)
                parts.Add(start.ToString("dd.MM.yy") + (EndDate is { } end ? $" — {end:dd.MM.yy}" : " — …"));
            if (ContractAmount > 0)
                parts.Add($"{ContractAmount:N0} руб.");
            return string.Join(" · ", parts);
        }
    }

}
