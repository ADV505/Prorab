namespace Core.Interfaces;

/// <summary>Колонка таблицы: заголовок и относительная ширина.</summary>
public record PdfColumn(string Header, float Weight = 1f);

/// <summary>Запрос на экспорт табличных данных в PDF.</summary>
public record PdfTableRequest(
    string Title,
    string? Subtitle,
    IReadOnlyList<PdfColumn> Columns,
    IEnumerable<IReadOnlyList<string>> Rows,
    string? FooterNote = null);

public interface IPdfReportService
{
    /// <summary>Рендерит таблицу в PDF и возвращает байты документа.</summary>
    byte[] ExportTable(PdfTableRequest request);
}
