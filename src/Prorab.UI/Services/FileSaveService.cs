using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Prorab.UI.Services
{
    public class FileSaveService(Func<TopLevel?> topLevel) : IFileSaveService
    {
        public async Task<string?> SavePdfAsync(string suggestedFileName)
        {
            var top = topLevel();
            if (top is null) return null;

            var file = await top.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Сохранить PDF",
                SuggestedFileName = suggestedFileName,
                DefaultExtension = "pdf",
                FileTypeChoices = [new FilePickerFileType("PDF") { Patterns = ["*.pdf"] }]
            });
            return file?.Path.LocalPath;
        }

        public async Task PrintAsync(byte[] pdf, string fileName)
        {
            var temp = Path.Combine(Path.GetTempPath(),
                $"Prorab_{DateTime.Now:yyyyMMdd_HHmmss}_{fileName}");
            await File.WriteAllBytesAsync(temp, pdf);

            // Открываем системный просмотрщик — там пользователь печатает сам
            Process.Start(new ProcessStartInfo(temp) { UseShellExecute = true });
        }
    }
}
