namespace Prorab.Core.Models;

public class WorkType
{
    /// <summary>Id</summary>
    public int Id { get; set; }

    /// <summary>Имя</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Единица измерения: м², м³, шт., смена...</summary>
    public string? Unit { get; set; }

    /// <summary>Базовая цена за единицу измерения, руб. Подставляется в наряд как значение по умолчанию.</summary>
    public decimal BasePrice { get; set; }

    /// <summary>В архиве или нет</summary>
    public bool IsArchived { get; set; }

    /// <summary>Строка «единица · цена» для отображения в списке.</summary>
    public string PriceInfo => BasePrice > 0
        ? $"{(string.IsNullOrEmpty(Unit) ? "" : Unit + " · ")}{BasePrice:N2} руб."
        : Unit ?? string.Empty;
}
