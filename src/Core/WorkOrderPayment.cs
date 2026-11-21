namespace Prorab.Core.Models;

/// <summary>Выплата по наряду конкретному работнику: аванс, зарплата или полный расчёт.</summary>
public class WorkOrderPayment
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }

    public int WorkerId { get; set; }
    public Worker? Worker { get; set; }

    public PaymentType Type { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public string? Comment { get; set; }
}
