using Lagoon.UI.Demo.Components;

namespace Lagoon.UI.Demo.Pages;

[AllowAnonymous()]
public partial class DemoPage : LgPage, ISourceCodeProvider
{
    #region constants

    protected const string DocumentationLinkBase = "https://doc.desirade.fr/?p=LagoonH-art~";

    #endregion

    #region fields

    private string _sourceName;

    #endregion

    #region properties

    public bool IsLoadingPreviewCode { get; set; } = true;
    public string BlazorContent { get; set; }
    public string CSharpContent { get; set; }
    protected string DocumentationComponent { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Get the source provdider.
    /// </summary>
    /// <param name="filePath">File name set by the compiler (leave empty).</param>
    /// <returns>The source provider.</returns>
    protected ISourceCodeProvider GetSourceCodeProvider([System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null)
    {
        _sourceName = System.IO.Path.GetFileName(callerFilePath).Split('.')[0];
        return this;
    }

    async Task ISourceCodeProvider.DownloadSourcesAsync()
    {
        try
        {
            BlazorContent = await DownloadPageSourceAsync($"{_sourceName}.razor");
            CSharpContent = await DownloadPageSourceAsync($"{_sourceName}.razor.cs");
        }
        finally
        {
            IsLoadingPreviewCode = false;
        }
    }
    private async Task<string> DownloadPageSourceAsync(string filename)
    {
        try
        {
            StringBuilder code = new(await AnonymousHttpClient.GetStringAsync($"_content/Lagoon.UI.Demo/sources/Pages/{filename}.txt"));
            code.Replace("<SourceCode SourceCodeProvider=\"GetSourceCodeProvider()\" />", "");
            code.Replace("@inherits DemoPage", "");
            code.Replace("@inherits DemoPageForm", "");
            return code.ToString().Trim('\n', '\r', '\t', ' ');
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }

    protected string DocumentationLink()
    {
        return $"{DocumentationLinkBase}{DocumentationComponent}";
    }

    #endregion

}
