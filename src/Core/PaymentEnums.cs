namespace Prorab.Core.Models;

public enum PaymentType
{
    Advance = 0,      // Аванс
    Salary = 1,       // Зарплата (официальная, ручной ввод)
    FullPayment = 2   // Полный расчёт
}

public enum PaymentMethod
{
    Cash = 0,             // Наличные
    CardTransfer = 1,     // Перевод на карту
    BankTransfer = 2      // Безналичный перевод
}
