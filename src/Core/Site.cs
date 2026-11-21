using System;
using System.Collections.Generic;

namespace Prorab.Core.Models
{
    public class Site
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ContractNumber { get; set; }
        public decimal ContractAmount { get; set; }
        public bool IsArchived { get; set; }

        // Статус завершения
        public bool IsCompleted => EndDate is { } end && end.Date < DateTime.Today;

        /// <summary>
        /// Текст для бейджа: дата окончания, "Завершён" или "без срока".
        /// </summary>
        public string BadgeText
        {
            get
            {
                if (IsCompleted)
                    return "Завершён";

                if (EndDate is { } end)
                    return $"до {end:dd.MM.yy}";

                return "без срока";
            }
        }

        /// <summary>
        /// Формирует строку "Договор: №... · Сумма" или просто "Договор: №...",
        /// избегая дублирования слова "Договор", если оно уже есть в ContractNumber.
        /// </summary>
        public string Info
        {
            get
            {
                var parts = new List<string>();

                if (!string.IsNullOrWhiteSpace(ContractNumber))
                {
                    var number = ContractNumber.Trim();
                    // Если пользователь уже ввел "Договор: ..." или "Договор №...", не добавляем префикс
                    if (number.StartsWith("Договор", StringComparison.OrdinalIgnoreCase))
                        parts.Add(number);
                    else
                        parts.Add($"Договор: {number}");
                }

                if (ContractAmount > 0)
                {
                    parts.Add($"{ContractAmount:N0} руб.");
                }

                return string.Join(" · ", parts);
            }
        }

        /// <summary>
        /// Флаг, есть ли что отображать в блоке информации (договор/сумма).
        /// Нужен, чтобы скрывать разделитель и пустую строку.
        /// </summary>
        public bool HasInfo => !string.IsNullOrEmpty(Info);
    }
}
