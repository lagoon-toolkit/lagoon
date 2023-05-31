namespace Lagoon.UI.Demo.Components;

public interface ISourceCodeProvider
{

    public bool IsLoadingPreviewCode { get; set; }
    public string BlazorContent { get; set; }
    public string CSharpContent { get; set; }

    Task DownloadSourcesAsync();

}
