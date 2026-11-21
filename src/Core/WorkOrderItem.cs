namespace Prorab.Core.Models;

/// <summary>Позиция наряда: вид работ в конкретном помещении.</summary>
public class WorkOrderItem
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }

    public int RoomId { get; set; }
    public Room? Room { get; set; }

    public int WorkTypeId { get; set; }
    public WorkType? WorkType { get; set; }

    /// <summary>Количество в единицах измерения вида работ.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Цена за единицу, зафиксированная на момент наряда.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Надбавка на позицию, % (null — нет).</summary>
    public decimal? SurchargePercent { get; set; }

    /// <summary>Скидка на позицию, % (null — нет).</summary>
    public decimal? DiscountPercent { get; set; }

    /// <summary>Сумма строки с учётом правок позиции.</summary>
    public decimal LineTotal =>
        Math.Round(Quantity * UnitPrice
            * (1 + (SurchargePercent ?? 0) / 100m)
            * (1 - (DiscountPercent ?? 0) / 100m), 2);
}
