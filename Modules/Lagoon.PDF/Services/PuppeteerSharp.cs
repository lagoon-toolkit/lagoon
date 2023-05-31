using Lagoon.PDF.Helpers;
using PuppeteerSharp;

namespace Lagoon.PDF.Services;

/// <summary>
/// Export html to Pdf service
/// </summary>
public class PuppeteerSharp : IPuppeteerSharp
{

    /// <summary>
    /// Private option for PuppeteerSharp
    /// </summary>
    private static readonly LaunchOptions _launchOptions = new()
    {
        Headless = true,
        ExecutablePath = ""
    };

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="chromiumPath">Path of the folder containing the executable (chromium)</param>
    public PuppeteerSharp(string chromiumPath)
    {
        _launchOptions.ExecutablePath = chromiumPath;
    }

    /// <summary>
    /// Export html page to Pdf
    /// </summary>
    /// <param name="url">Url/Path of html page</param>
    /// <returns>Return Pdf as a stream</returns>
    public async Task<Stream> UrlToPdf(string url)
    {
        using (var browser = await Puppeteer.LaunchAsync(_launchOptions))
        using (var page = await browser.NewPageAsync())
        {
            await page.GoToAsync(url);
            return await page.PdfStreamAsync();
        }
    }

    /// <summary>
    /// Export html page to Pdf
    /// </summary>
    /// <param name="url">Url/Path of html page</param>
    /// <param name="pdfOptions">Pdf options</param>
    /// <returns>Return Pdf as a stream</returns>
    public async Task<Stream> UrlToPdf(string url, ExportPdfOptions pdfOptions)
    {
        using (var browser = await Puppeteer.LaunchAsync(_launchOptions))
        using (var page = await browser.NewPageAsync())
        {
            await page.GoToAsync(url);
            return await page.PdfStreamAsync(PdfOptionsHelper.GetPdfOptions(pdfOptions));
        }
    }

    /// <summary>
    /// Export html code to Pdf
    /// </summary>
    /// <param name="html">html code</param>
    /// <returns>Return Pdf as a stream</returns>
    public async Task<Stream> HtmlToPdf(string html)
    {
        using (var browser = await Puppeteer.LaunchAsync(_launchOptions))
        using (var page = await browser.NewPageAsync())
        {
            await page.SetContentAsync(html);
            return await page.PdfStreamAsync();
        }
    }

    /// <summary>
    /// Export html code to Pdf
    /// </summary>
    /// <param name="html">html code</param>
    /// <param name="pdfOptions">Pdf options</param>
    /// <returns>Return Pdf as a stream</returns>
    public async Task<Stream> HtmlToPdf(string html, ExportPdfOptions pdfOptions)
    {
        using (var browser = await Puppeteer.LaunchAsync(_launchOptions))
        using (var page = await browser.NewPageAsync())
        {
            await page.SetContentAsync(html);
            return await page.PdfStreamAsync(PdfOptionsHelper.GetPdfOptions(pdfOptions));
        }
    }

    /// <summary>
    /// Takes a screenshot of the page
    /// </summary>
    /// <param name="url">Url/Path of html page</param>
    /// <returns>Return jpeg image as a stream</returns>
    public async Task<Stream> UrlToJpg(string url)
    {
        using (var browser = await Puppeteer.LaunchAsync(_launchOptions))
        using (var page = await browser.NewPageAsync())
        {
            await page.GoToAsync(url);
            return await page.ScreenshotStreamAsync();
        }
    }

    /// <summary>
    /// Takes a screenshot of the html code
    /// </summary>
    /// <param name="html">html code</param>
    /// <returns>Return jpeg image as a stream</returns>
    public async Task<Stream> HtmlToJpg(string html)
    {
        using (var browser = await Puppeteer.LaunchAsync(_launchOptions))
        using (var page = await browser.NewPageAsync())
        {
            await page.SetContentAsync(html);
            return await page.ScreenshotStreamAsync();
        }
    }

    /// <summary>
    /// Ask to Download the chromium revision if necessary.
    /// </summary>
    /// <param name="revision">Chromium revision</param>
    /// <returns>Return revision info</returns>
    public static Task<RevisionInfo> DownloadAsync(string revision = BrowserFetcher.DefaultChromiumRevision)
    {
        return new BrowserFetcher().DownloadAsync(revision);
    }
}
