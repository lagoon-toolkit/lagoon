namespace Lagoon.PDF.Services;

/// <summary>
/// Export html to Pdf/Jpg interface
/// </summary>
public interface IPuppeteerSharp
{
    /// <summary>
    /// Export html page to Pdf
    /// </summary>
    /// <param name="url">Url/Path of html page</param>
    /// <returns>return Pdf as a stream</returns>
    Task<Stream> UrlToPdf(string url);

    /// <summary>
    /// Export html page to Pdf
    /// </summary>
    /// <param name="url">Url/Path of html page</param>
    /// <param name="pdfOptions">Pdf options</param>
    /// <returns>return Pdf as a stream</returns>
    Task<Stream> UrlToPdf(string url, ExportPdfOptions pdfOptions);

    /// <summary>
    /// Export html code to Pdf
    /// </summary>
    /// <param name="html">html code</param>
    /// <returns>return Pdf as a stream</returns>
    Task<Stream> HtmlToPdf(string html);

    /// <summary>
    /// Export html code to Pdf
    /// </summary>
    /// <param name="html">html code</param>
    /// <param name="pdfOptions">Pdf options</param>
    /// <returns>return Pdf as a stream</returns>
    Task<Stream> HtmlToPdf(string html, ExportPdfOptions pdfOptions);

    /// <summary>
    /// Takes a screenshot of the page
    /// </summary>
    /// <param name="url">Url/Path of html page</param>
    /// <returns>Return jpeg image as a stream</returns>
    Task<Stream> UrlToJpg(string url);

    /// <summary>
    /// Takes a screenshot of the html code
    /// </summary>
    /// <param name="html">html code</param>
    /// <returns>Return jpeg image as a stream</returns>
    Task<Stream> HtmlToJpg(string html);
}
