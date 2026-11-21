namespace Prorab.Core.Models;

/// <summary>Наряд.</summary>
public class WorkOrder
{
    public int Id { get; set; }

    public int CrewId { get; set; }
    public Crew? Crew { get; set; }

    public int SiteId { get; set; }
    public Site? Site { get; set; }

    /// <summary>Номер наряда (формируется при создании, например «12/22.11»).</summary>
    public string Number { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    /// <summary>Надбавка на весь наряд, % (0 — нет).</summary>
    public decimal SurchargePercent { get; set; }

    /// <summary>Скидка на весь наряд, % (0 — нет).</summary>
    public decimal DiscountPercent { get; set; }

    public string? Comment { get; set; }
    public bool IsArchived { get; set; }

    public List<WorkOrderItem> Items { get; set; } = [];
    public List<WorkOrderPayment> Payments { get; set; } = [];

    /// <summary>Сумма по позициям с учётом правок позиций.</summary>
    public decimal ItemsTotal =>
        Items.Sum(i => i.LineTotal);

    /// <summary>Итог к выплате: позиции + надбавка − скидка наряда.</summary>
    public decimal GrandTotal =>
        Math.Round(ItemsTotal * (1 + SurchargePercent / 100m) * (1 - DiscountPercent / 100m), 2);

    /// <summary>Уже выплачено по наряду.</summary>
    public decimal PaidTotal => Payments.Sum(p => p.Amount);

    /// <summary>Остаток к выплате.</summary>
    public decimal Remaining => GrandTotal - PaidTotal;
}
