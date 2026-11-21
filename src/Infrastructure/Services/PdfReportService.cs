using Core.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Services;

public class PdfReportService : IPdfReportService
{
    static PdfReportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.UseSystemFonts = true;
    }

    public byte[] ExportTable(PdfTableRequest request) =>
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Segoe UI"));

                page.Header().Column(col =>
                {
                    col.Item().Text(request.Title)
                        .FontSize(18).Bold().FontColor(Colors.Grey.Darken3);
                    if (request.Subtitle is not null)
                        col.Item().PaddingTop(2).Text(request.Subtitle)
                            .FontSize(10).FontColor(Colors.Grey.Darken2);
                    col.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        foreach (var c in request.Columns)
                            columns.RelativeColumn(c.Weight);
                    });

                    table.Header(header =>
                    {
                        foreach (var c in request.Columns)
                            header.Cell().Background("#2E7CD6").Padding(6)
                                  .Text(c.Header).FontColor(Colors.White).SemiBold();
                    });

                    var zebra = false;
                    foreach (var row in request.Rows)
                    {
                        for (var i = 0; i < request.Columns.Count; i++)
                        {
                            var cell = zebra
                                ? table.Cell().Background("#F1F4F8")
                                : table.Cell();

                            cell.Padding(6).Text(i < row.Count ? row[i] : string.Empty);
                        }
                        zebra = !zebra;
                    }

                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text(request.FooterNote ?? string.Empty)
                        .FontSize(8).FontColor(Colors.Grey.Darken1);
                    row.AutoItem().Text(x =>
                    {
                        x.Span("Стр. ").FontSize(8);
                        x.CurrentPageNumber().FontSize(8);
                        x.Span(" из ").FontSize(8);
                        x.TotalPages().FontSize(8);
                    });
                });
            });
        }).GeneratePdf();
}

