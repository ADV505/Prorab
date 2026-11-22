using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Prorab.Core.Models;
using Prorab.Infrastructure.Data;
using Prorab.UI.ViewModels;

namespace Prorab.UI.Modules.WorkOrders;

/// <summary>Обёртка значения enum с человеческой подписью.</summary>
public record KindItem<T>(T Value, string Label)
{
    public override string ToString() => Label;
}

/// <summary>Строка списка выплат.</summary>
public partial class PaymentRow : ObservableObject
{
    public int Id { get; init; }
    public string WorkerName { get; init; } = "";
    public string TypeText { get; init; } = "";
    public string MethodText { get; init; } = "";
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
    public string Comment { get; init; } = "";

    public string DateText => Date.ToString("dd.MM.yyyy");
    public string AmountText => Amount.ToString("0.##");
}

public partial class WorkOrderPaymentsViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private int _orderId;
    private Crew? _crew;

    public ObservableCollection<Worker> Workers { get; } = [];
    public ObservableCollection<KindItem<PaymentType>> Types { get; } =
        [.. Enum.GetValues<PaymentType>().Select(t => new KindItem<PaymentType>(t, TypeLabel(t)))];
    public ObservableCollection<KindItem<PaymentMethod>> Methods { get; } =
        [.. Enum.GetValues<PaymentMethod>().Select(m => new KindItem<PaymentMethod>(m, MethodLabel(m)))];

    public ObservableCollection<PaymentRow> Payments { get; } = [];

    [ObservableProperty] private Worker? _selectedWorker;
    [ObservableProperty] private KindItem<PaymentType> _selectedType;
    [ObservableProperty] private KindItem<PaymentMethod> _selectedMethod;
    [ObservableProperty] private string _amountText = "";
    [ObservableProperty] private string _commentText = "";
    [ObservableProperty] private string _statusText = "";

    public string Title { get; private set; } = "";
    public string SummaryText { get; private set; } = "";

    public WorkOrderPaymentsViewModel(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
        _selectedType = Types.Count > 0 ? Types[0] : new(PaymentType.Advance, TypeLabel(PaymentType.Advance));
        _selectedMethod = Methods.Count > 0 ? Methods[0] : new(PaymentMethod.Cash, MethodLabel(PaymentMethod.Cash));
    }

    private static string TypeLabel(PaymentType type) => type switch
    {
        PaymentType.Advance => "Аванс",
        PaymentType.Salary => "Зарплата",
        PaymentType.FullPayment => "Полный расчёт",
        _ => type.ToString(),
    };

    private static string MethodLabel(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash => "Наличные",
        PaymentMethod.CardTransfer => "Перевод на карту",
        PaymentMethod.BankTransfer => "Безналичный перевод",
        _ => method.ToString(),
    };

    partial void OnSelectedTypeChanged(KindItem<PaymentType> value)
    {
        RebuildWorkers();

        // Зарплата официально устроенным уходит безналом — выставляем способ сразу
        if (value?.Value == PaymentType.Salary)
            SelectedMethod = Methods.FirstOrDefault(m => m.Value == PaymentMethod.BankTransfer)
                             ?? SelectedMethod;

    }

    /// <summary>Список на выплату зависит от типа: аванс/полный расчёт — бригадир,
    /// зарплата — официально устроенные (физлица по договору подряда).</summary>
    private void RebuildWorkers()
    {
        Workers.Clear();

        IEnumerable<Worker> source;
        if (SelectedType is { } t && t.Value is PaymentType.Advance or PaymentType.FullPayment)
        {
            source = _crew?.Foreman is { IsArchived: false } f ? [f] : [];
        }
        else
        {
            source = _crew?.Members
                .Select(m => m.Worker)
                .OfType<Worker>()
                .Where(w => w.IsHired)
                ?? [];
        }

        foreach (var w in source.DistinctBy(w => w.Id).OrderBy(w => w.FullName))
            Workers.Add(w);

        SelectedWorker = Workers.FirstOrDefault();
    }

    public async Task LoadAsync(int orderId)
    {
        _orderId = orderId;

        await using var db = await _factory.CreateDbContextAsync();
        await DbMigration.EnsureMigratedAsync(_factory);

        var order = await db.WorkOrders.FirstAsync(o => o.Id == orderId);
        _crew = await db.Crews
            .Include(c => c.Foreman)
            .Include(c => c.Members).ThenInclude(m => m.Worker)
            .FirstOrDefaultAsync(c => c.Id == order.CrewId);

        await RefreshAsync();
        RebuildWorkers();
    }

    private async Task RefreshAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        var order = await db.WorkOrders
            .Include(o => o.Items)
            .Include(o => o.Payments).ThenInclude(p => p.Worker)
            .FirstAsync(o => o.Id == _orderId);

        // Безнал официально устроенным — эта часть уже вычтена из остатка,
        // показываем её отдельно, чтобы было видно, из чего сложилась выплата
        var official = order.Payments
            .Where(p => p.Method == PaymentMethod.BankTransfer)
            .Sum(p => p.Amount);

        Title = $"Выплаты — наряд №{order.Number}";
        OnPropertyChanged(nameof(Title));
        SummaryText = $"Сумма {order.GrandTotal:0.##} · Выплачено {order.PaidTotal:0.##}" +
                      (official > 0 ? $" (безнал {official:0.##})" : "") +
                      $" · Остаток {order.Remaining:0.##}";
        OnPropertyChanged(nameof(SummaryText));

        Payments.Clear();
        foreach (var p in order.Payments.OrderByDescending(p => p.Date))
        {
            Payments.Add(new PaymentRow
            {
                Id = p.Id,
                WorkerName = p.Worker?.FullName ?? "?",
                TypeText = TypeLabel(p.Type),
                MethodText = MethodLabel(p.Method),
                Amount = p.Amount,
                Date = p.Date,
                Comment = p.Comment ?? "",
            });
        }
    }

    public async Task AddPaymentAsync()
    {
        try
        {
            StatusText = "";
            if (Workers.Count == 0)
            {
                StatusText = SelectedType?.Value == PaymentType.Salary
                    ? "В бригаде нет официально устроенных работников."
                    : "У бригады не назначен бригадир — назначьте его в справочнике бригад.";
                return;
            }
            if (SelectedWorker is null || !Workers.Contains(SelectedWorker))
            {
                StatusText = "В бригаде нет работников с оформленным договором подряда.";
                return;
            }
            if (!decimal.TryParse(AmountText, out var amount) || amount <= 0)
            {
                StatusText = "Укажите сумму больше нуля.";
                return;
            }

            await using var db = await _factory.CreateDbContextAsync();
            var order = await db.WorkOrders
                .Include(o => o.Items)
                .Include(o => o.Payments)
                .FirstAsync(o => o.Id == _orderId);


            if (amount > order.Remaining)
            {
                StatusText = $"Остаток к выплате {order.Remaining:0.##} — сумма больше.";
                return;
            }

            db.WorkOrderPayments.Add(new WorkOrderPayment
            {
                WorkOrderId = _orderId,
                WorkerId = SelectedWorker!.Id,
                Type = SelectedType!.Value,
                Method = SelectedMethod!.Value,
                Amount = amount,
                Date = DateTime.Today,
                Comment = CommentText.Trim(),
            });
            await db.SaveChangesAsync();

            AmountText = "";
            CommentText = "";
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusText = "Ошибка: " + ex.Message;
            if (ex.InnerException is { } inner)
                StatusText += " — " + inner.Message;
        }
    }


    public async Task DeletePaymentAsync(PaymentRow row)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var payment = await db.WorkOrderPayments.FindAsync(row.Id);
        if (payment is null) return;
        db.WorkOrderPayments.Remove(payment);
        await db.SaveChangesAsync();
        await RefreshAsync();
    }
}
