using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Diagnostics;

namespace Prorab.UI.Services;

public interface IFileSaveService
{
    /// <summary>Диалог «Сохранить как». null — пользователь отменил.</summary>
    Task<string?> SavePdfAsync(string suggestedFileName);

    /// <summary>Готовит PDF и открывает системный просмотрщик для печати.</summary>
    Task PrintAsync(byte[] pdf, string fileName);
}



