using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Lagoon.PDF.Helpers;

/// <summary>
/// Pdf options helper
/// </summary>
static class PdfOptionsHelper
{
    /// <summary>
    /// Convert to PdfOptions
    /// </summary>
    /// <param name="exportPdfOptions"></param>
    /// <returns>PdfOptions</returns>
    public static PdfOptions GetPdfOptions(ExportPdfOptions exportPdfOptions)
    {
        var pdfOptions = new PdfOptions
        {
            Scale = exportPdfOptions.Scale,
            DisplayHeaderFooter = exportPdfOptions.DisplayHeaderFooter,
            HeaderTemplate = exportPdfOptions.HeaderTemplate,
            FooterTemplate = exportPdfOptions.FooterTemplate,
            PrintBackground = exportPdfOptions.PrintBackground,
            Landscape = exportPdfOptions.Landscape,
            PageRanges = exportPdfOptions.PageRanges,
            Format = GetPaperFormat(exportPdfOptions.Format),
            MarginOptions = new MarginOptions
            {
                Top = exportPdfOptions.MarginOptions.Top,
                Bottom = exportPdfOptions.MarginOptions.Bottom,
                Right = exportPdfOptions.MarginOptions.Right,
                Left = exportPdfOptions.MarginOptions.Left
            }
        };

        return pdfOptions;
    }

    /// <summary>
    /// Convert to PaperFormat
    /// </summary>
    /// <param name="format">PaperFormatEnum</param>
    /// <returns>PaperFormat</returns>
    public static PaperFormat GetPaperFormat(PaperFormatEnum format)
    {
        return format switch
        {
            PaperFormatEnum.Letter => PaperFormat.Letter,
            PaperFormatEnum.Legal => PaperFormat.Legal,
            PaperFormatEnum.Tabloid => PaperFormat.Tabloid,
            PaperFormatEnum.Ledger => PaperFormat.Ledger,
            PaperFormatEnum.A0 => PaperFormat.A0,
            PaperFormatEnum.A1 => PaperFormat.A1,
            PaperFormatEnum.A2 => PaperFormat.A2,
            PaperFormatEnum.A3 => PaperFormat.A3,
            PaperFormatEnum.A4 => PaperFormat.A4,
            PaperFormatEnum.A5 => PaperFormat.A5,
            PaperFormatEnum.A6 => PaperFormat.A6,
            _ => PaperFormat.A4,
        };
    }
}
